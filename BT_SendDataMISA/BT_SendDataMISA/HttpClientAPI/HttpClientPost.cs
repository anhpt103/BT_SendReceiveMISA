using FluentResults;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BT_SendDataMISA.HttpClientAPI
{
    public class HttpClientPost
    {
        public async Task<Result> SendsRequest(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "BOn8WT6PNbf3vWHsITrznIWyaZ7QhISp");
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.OK) return Result.Ok(response.Content.ReadAsStringAsync().Result);
                        else return Result.Fail("Phản hồi từ API, mã lỗi: " + response.StatusCode);
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
