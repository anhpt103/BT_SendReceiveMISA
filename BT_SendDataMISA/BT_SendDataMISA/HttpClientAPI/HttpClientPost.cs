﻿using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BT_SendDataMISA.HttpClientAPI
{
    public class HttpClientPost
    {
        public async Task<Result> SendsRequest(string url, object obj)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    var keyValues = new List<KeyValuePair<string, string>>();

                    Dictionary<string, object> dict = obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null));
                    foreach (var kv in dict)
                    {
                        keyValues.Add(new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()));
                    }

                    request.Content = new FormUrlEncodedContent(keyValues);
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode != HttpStatusCode.OK) return Result.Fail("Phản hồi từ API, mã lỗi: " + response.StatusCode);
                        else
                        {
                            string result = response.Content.ReadAsStringAsync().Result;
                            if (result.Length == 0) Result.Fail("Kết quả trả về từ API rỗng");

                            return Result.Ok().WithSuccess(result);
                        }
                    }
                    else return Result.Fail("Xảy ra ngoại lệ: " + response.StatusCode);
                }
                catch (ArgumentNullException ex)
                {
                    return Result.Fail("Giá trị tham số không đúng: " + ex.Message);
                }
                catch (HttpRequestException ex)
                {
                    return Result.Fail("Xảy ra Exception khi gọi API: " + ex.Message);
                }
            }
        }

        public async Task<Result> SendsRequestWithToken(string url, string token = "", object obj = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                    if (!string.IsNullOrEmpty(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    if (obj != null)
                    {
                        var keyValues = new List<KeyValuePair<string, string>>();

                        Dictionary<string, object> dict = obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null));
                        foreach (var kv in dict)
                        {
                            keyValues.Add(new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()));
                        }
                        request.Content = new FormUrlEncodedContent(keyValues);
                    }

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode != HttpStatusCode.OK) return Result.Fail("Phản hồi từ API, mã lỗi: " + response.StatusCode);
                        else
                        {
                            string result = response.Content.ReadAsStringAsync().Result;
                            if (result.Length == 0) Result.Fail("Kết quả trả về từ API rỗng");

                            return Result.Ok().WithSuccess(result);
                        }
                    }
                    else return Result.Fail("Xảy ra ngoại lệ: " + response.StatusCode);
                }
                catch (ArgumentNullException ex)
                {
                    return Result.Fail("Giá trị tham số không đúng: " + ex.Message);
                }
                catch (HttpRequestException ex)
                {
                    return Result.Fail("Xảy ra Exception khi gọi API: " + ex.Message);
                }
            }
        }
    }
}
