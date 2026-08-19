using RENT_MVC_PROJECT.DTO;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace RENT_MVC_PROJECT.Repository
{
    public class PostDataRepo
    {
        private readonly empDto _dto;

        public PostDataRepo(empDto dto)
        {
            _dto = dto;
        }




        public string RemoveSpecialCharacters(string str)
        {
            //-._~+/
            //  return Regex.Replace(str, "[^a-zA-Z0-9_~+/-]+", "", RegexOptions.Compiled);

            return Regex.Replace(str, "[^a-zA-Z0-9_~+/-{}]+", "", RegexOptions.Compiled);

        }


        public async Task<dynamic> PostInternalPageData(string indata, string flag, string baseurl, string ApiPath)
        {
            string data = "";
            using (HttpClient client = new HttpClient())
            {

                string content = JsonConvert.SerializeObject(new { p_pagevalue = indata, p_paravalue = "1", p_flag = flag });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                string url2 = baseurl + ApiPath;
                HttpResponseMessage response2 = await client.PostAsync(url2, byteContent);
                if (response2.IsSuccessStatusCode)
                {
                    data = response2.Content.ReadAsStringAsync().Result;
                    data = data.Replace("\"\"", "");
                   // data = data.Replace("\'", "");
                    data = data.Replace(@"""RESP"":", @"");
                   // data = data.Replace("", "");
                  
                }

                //response2.EnsureSuccessStatusCode();
                //string responseBody2 = await response2.Content.ReadAsStringAsync();
                // return responseBody2;

            }

            //data = RemoveSpecialCharacters(data);
            return data;
        }

          

        public async Task<dynamic> PostInternalPageData1(string indata, string input1, string input2, string input3, string input4, string flag, string baseurl, string ApiPath)
        {
            string data = "";
            using (HttpClient client = new HttpClient())
            {

                string content = JsonConvert.SerializeObject(new { p_pagevalue = indata, p_paravalue = "1", p_flag = flag });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                string url2 = baseurl + ApiPath;
                HttpResponseMessage response2 = await client.PostAsync(url2, byteContent);
                if (response2.IsSuccessStatusCode)
                {
                    data = response2.Content.ReadAsStringAsync().Result;
                    data = data.Replace("\"\"", "");
                    // data = data.Replace("\'", "");
                    data = data.Replace(@"""RESP"":", @"");
                    // data = data.Replace("", "");

                }

                //response2.EnsureSuccessStatusCode();
                //string responseBody2 = await response2.Content.ReadAsStringAsync();
                // return responseBody2;

            }

            //data = RemoveSpecialCharacters(data);
            return data;
        }



        public async Task<dynamic> ApiSessionDecrypt(string indata, string baseurl, string ApiPath)
        {
            string data = "";
            using (HttpClient client = new HttpClient())
            {

                string content = JsonConvert.SerializeObject(new { data = indata });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                string url2 = baseurl + ApiPath;
                HttpResponseMessage response2 = await client.PostAsync(url2, byteContent);
                if (response2.IsSuccessStatusCode)
                {
                    data = response2.Content.ReadAsStringAsync().Result;

                    //data = data.Replace("\"\"", "");
                    //// data = data.Replace("\'", "");
                    //data = data.Replace(@"""RESP"":", @"");
                    //// data = data.Replace("", "");

                }

                //response2.EnsureSuccessStatusCode();
                //string responseBody2 = await response2.Content.ReadAsStringAsync();
                // return responseBody2;

            }

            //data = RemoveSpecialCharacters(data);
            return data;
        }


        public async Task<dynamic> UploadDocument(string query, string code, string baseurl, string ApiPath)
        {
            string data = "";
            using (HttpClient client = new HttpClient())
            {

                string content = JsonConvert.SerializeObject(new { p_query = query, docData = code });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                string url2 = baseurl + ApiPath;
                HttpResponseMessage response2 = await client.PostAsync(url2, byteContent);
                if (response2.IsSuccessStatusCode)
                {
                    data = response2.Content.ReadAsStringAsync().Result;
                    data = data.Replace("\"\"", "");
                    data = data.Replace("\'", "");
                   // data = data.Replace(@"""RESP"":", @"");

                }


            }

            data = RemoveSpecialCharacters(data);

            return data;

        }


        public async Task<dynamic> PANHelperData(string PAN_NO, string empcode, string fid, string ApiPath)
{
            string data = "";
            using (HttpClient client = new HttpClient())
            {

        string content = JsonConvert.SerializeObject(new { pan = PAN_NO, firmid = fid, empid = empcode });
        var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                string url2 =ApiPath;
                HttpResponseMessage response2 = await client.PostAsync(url2, byteContent);

                //

                if (response2.IsSuccessStatusCode)
                {
                    data = await response2.Content.ReadAsStringAsync().ConfigureAwait(false);
                    // data = response2.Content.ReadAsStringAsync().Result;
                    // data = data.Replace("\"\"", "");
                    //data = data.Replace("\'", "");

                }

            }

           // data = RemoveSpecialCharacters(data);

            return data;
        }


    }
}



