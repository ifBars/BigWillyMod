using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using MelonLoader;
using S1API.Entities;
using UnityEngine;
using BigWillyMod.NPCs;
using BigWillyMod.Utils;

namespace BigWillyMod.Services
{
    /// <summary>
    /// Service that periodically checks if Big Willy is live on Twitch
    /// and sends an in-game text notification when he goes live.
    /// </summary>
    public static class LiveStreamChecker
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static object? _pollingCoroutine;
        private static bool _wasLiveLastCheck = false;
        private static bool _notifiedThisSession = false;

        static LiveStreamChecker()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Starts the live stream polling coroutine.
        /// </summary>
        public static void StartPolling()
        {
            if (_pollingCoroutine != null)
            {
                DebugLog.Msg("[LiveStreamChecker] Polling already running");
                return;
            }

            if (!Core.LiveNotificationsEnabled)
            {
                DebugLog.Msg("[LiveStreamChecker] Live notifications disabled, not starting polling");
                return;
            }

            _pollingCoroutine = MelonCoroutines.Start(PollStreamStatus());
            DebugLog.Msg("[LiveStreamChecker] Started polling for live streams");
        }

        /// <summary>
        /// Stops the live stream polling coroutine.
        /// </summary>
        public static void StopPolling()
        {
            if (_pollingCoroutine != null)
            {
                MelonCoroutines.Stop(_pollingCoroutine);
                _pollingCoroutine = null;
                DebugLog.Msg("[LiveStreamChecker] Stopped polling");
            }
        }

        /// <summary>
        /// Resets the notification state (call when returning to menu).
        /// </summary>
        public static void Reset()
        {
            _wasLiveLastCheck = false;
            _notifiedThisSession = false;
        }

        private static IEnumerator PollStreamStatus()
        {
            // Initial delay before first check (let game fully load)
            yield return new WaitForSeconds(10f);

            while (true)
            {
                // Check stream status
                var checkTask = CheckTwitchLiveAsync();

                // Wait for the async task to complete
                while (!checkTask.IsCompleted)
                {
                    yield return null;
                }

                bool isLive = false;
                try
                {
                    isLive = checkTask.Result;
                }
                catch (Exception ex)
                {
                    DebugLog.Msg($"[LiveStreamChecker] Error checking stream status: {ex.Message}");
                }

                // Handle state transitions
                if (isLive && !_wasLiveLastCheck)
                {
                    // Just went live!
                    DebugLog.Msg("[LiveStreamChecker] Stream just went live!");

                    if (!_notifiedThisSession)
                    {
                        SendLiveNotification();
                        _notifiedThisSession = true;
                    }
                }
                else if (!isLive && _wasLiveLastCheck)
                {
                    // Stream ended - reset notification flag for next stream
                    DebugLog.Msg("[LiveStreamChecker] Stream ended, resetting notification flag");
                    _notifiedThisSession = false;
                }

                _wasLiveLastCheck = isLive;

                // Wait for next check interval
                float intervalSeconds = Core.LiveCheckIntervalMinutes * 60f;
                yield return new WaitForSeconds(intervalSeconds);
            }
        }

        private static async Task<bool> CheckTwitchLiveAsync()
        {
            try
            {
                string response = await _httpClient.GetStringAsync(Constants.LiveStream.CHECK_URL);
                string trimmed = response.Trim();

                if (trimmed.Equals("offline", StringComparison.OrdinalIgnoreCase))
                {
                    DebugLog.Msg("[LiveStreamChecker] Stream check: offline");
                    return false;
                }

                string[] uptimeIndicators = { "second", "minute", "hour" };
                bool onlineUptime = Array.Exists(uptimeIndicators, 
                    indicator => trimmed.Contains(indicator, StringComparison.OrdinalIgnoreCase));

                if (onlineUptime)
                {
                    DebugLog.Msg($"[LiveStreamChecker] Stream check: LIVE (uptime: {trimmed})");
                    return true;
                }

                DebugLog.Msg($"[LiveStreamChecker] Unexpected response, assuming offline: {trimmed}");
                return false;
            }
            catch (HttpRequestException ex)
            {
                DebugLog.Error($"[LiveStreamChecker] HTTP error checking Twitch: {ex.Message}");
                DebugLog.Error($"[LiveStreamChecker] Stack trace: {ex.StackTrace}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                DebugLog.Error($"[LiveStreamChecker] Twitch check timed out: {ex.Message}");
                DebugLog.Error($"[LiveStreamChecker] Stack trace: {ex.StackTrace}");
                return false;
            }
            catch (Exception ex)
            {
                DebugLog.Error($"[LiveStreamChecker] Unexpected error: {ex.Message}");
                DebugLog.Error($"[LiveStreamChecker] Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private static void SendLiveNotification()
        {
            try
            {
                var bigWilly = NPC.Get<BigWilly>();
                if (bigWilly == null)
                {
                    DebugLog.Error("[LiveStreamChecker] BigWilly NPC not found, cannot send notification");
                    return;
                }

                string message = $"Yo! I'm live right now, come hang out! {Constants.LiveStream.TWITCH_URL}";
                bigWilly.SendTextMessage(message);

                DebugLog.Msg("[LiveStreamChecker] Sent live notification to player");
            }
            catch (Exception ex)
            {
                DebugLog.Error($"[LiveStreamChecker] Failed to send notification: {ex.Message}");
                DebugLog.Error($"[LiveStreamChecker] Stack trace: {ex.StackTrace}");
            }
        }
    }
}
