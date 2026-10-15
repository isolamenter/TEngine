using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace TEngine
{
    public static partial class Utility
    {
        public static partial class Http
        {
            public enum JsonRequestStatus { Success, HttpError, TransportError, Timeout }

            public readonly struct JsonResponse
            {
                public readonly JsonRequestStatus Status;
                public readonly long StatusCode;
                public readonly string Body;

                public JsonResponse(JsonRequestStatus status, long statusCode, string body)
                {
                    Status = status;
                    StatusCode = statusCode;
                    Body = body;
                }
            }

            // 不重试提交。业务错误由项目根据响应 DTO 判定，取消按异常传回调用方。
            public static async UniTask<JsonResponse> SendJsonAsync(string url, string method,
                string json = null, IReadOnlyDictionary<string, string> headers = null,
                float timeoutSeconds = 15f, CancellationToken cancellationToken = default)
            {
                if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("URL is required.", nameof(url));
                if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("HTTP method is required.", nameof(method));
                if (timeoutSeconds <= 0 || float.IsNaN(timeoutSeconds) || float.IsInfinity(timeoutSeconds))
                    throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));
                cancellationToken.ThrowIfCancellationRequested();

                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var timeoutTimer = timeout.CancelAfterSlim(TimeSpan.FromSeconds(timeoutSeconds), DelayType.Realtime);
                using var request = new UnityWebRequest(url, method);
                request.downloadHandler = new DownloadHandlerBuffer();
                if (headers != null)
                    foreach (var header in headers) request.SetRequestHeader(header.Key, header.Value);
                if (json != null)
                {
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                    request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                }

                try
                {
                    var operation = request.SendWebRequest();
                    // 不让 await 扩展把 HTTP 错误转为异常；响应 body 在本方法释放 handler 前读取。
                    await UniTask.WaitUntil(() => operation.isDone, cancellationToken: timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    request.Abort();
                    cancellationToken.ThrowIfCancellationRequested();
                    return new JsonResponse(JsonRequestStatus.Timeout, request.responseCode, null);
                }
                cancellationToken.ThrowIfCancellationRequested();
                var status = request.result == UnityWebRequest.Result.Success
                    ? JsonRequestStatus.Success
                    : request.result == UnityWebRequest.Result.ProtocolError
                        ? JsonRequestStatus.HttpError : JsonRequestStatus.TransportError;
                return new JsonResponse(status, request.responseCode, request.downloadHandler.text);
            }
        }
    }
}
