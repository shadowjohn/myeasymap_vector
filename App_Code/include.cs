using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Data;
using System.Data.SQLite;
using System.IO.Compression;
using Ionic.Zip;


namespace utility
{
    public class myinclude : System.Web.Services.WebService
    {
        public string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";

        public Dictionary<string, object> getGET_POST(string inputs, string method)
        {
            /*
                string GETS_STRING="mode,id";
                Dictionary<string, object> get = x.getGET_POST(GETS_STRING, "GET");
                string vv=x.print_r(get,0);
                foreach (string a in get.Keys)
                {
                    Response.Write(a+":"+get[a]+"<br>");
                }
               sample:
                string GETS_STRING="mode,id";
                Dictionary<string, object> get = x.getGET_POST(GETS_STRING, "GET");

                string POSTS_STRING ="abc,b,s_a,s_b[],ddd";

                Dictionary<string, object> post = x.getGET_POST(POSTS_STRING, "POST");
                string q = x.print_r(get, 0);
                string p = x.print_r(post, 0);
                Response.Write("<pre>" + q + "<br>" + p + "</pre>");
                Response.Write("aaaaaaa->" + post["s_a"]+"<br>");
                Response.Write("aaaaaab->" + post["s_b[]"] + "<br>");             
             * 
            */
            method = method.ToUpper();
            Dictionary<string, object> get_post = new Dictionary<string, object>();
            switch (method)
            {
                case "REQUEST":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.QueryString[k] != null)
                        {
                            get_post[k] = this.Context.Request.QueryString[k];
                        }
                        else if (this.Context.Request.Form[k] != null)
                        {
                            get_post[k] = this.Context.Request.Form[k];
                        }
                        else
                        {
                            get_post[k] = "";
                        }
                    }
                    break;
                case "GET":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.QueryString[k] != null)
                        {
                            get_post[k] = this.Context.Request.QueryString[k];
                        }
                        else
                        {
                            get_post[k] = "";
                        }

                    }
                    break;
                case "POST":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.Form[k] != null)
                        {
                            if (this.Context.Request.Form.GetValues(k).Length != 1)
                            {
                                //暫時先這樣，以後再修= =
                                //alert(this.Context.Request.Form.GetValues(k).Length.ToString());
                                get_post[k] = implode("┃", this.Context.Request.Form.GetValues(k));
                            }
                            else
                            {
                                get_post[k] = this.Context.Request.Form[k];
                            }
                        }
                        else
                        {
                            get_post[k] = "";
                        }
                    }
                    break;
                case "POST_UNVALID":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.Unvalidated[k] != null)
                        {
                            if (this.Context.Request.Unvalidated.Form.GetValues(k).Length > 0)
                            {
                                //暫時先這樣，以後再修= =
                                //alert(this.Context.Request.Form.GetValues(k).Length.ToString());
                                get_post[k] = implode("┃", this.Context.Request.Unvalidated.Form.GetValues(k));
                            }
                            else
                            {
                                //get_post[k] = this.Context.Request.Form[k];
                                get_post[k] = this.Context.Request.Unvalidated.Form[k];
                            }

                        }
                        else
                        {
                            get_post[k] = "";
                        }
                    }
                    break;
            }
            return get_post;
        }
        public string implode(string keyword, string[] arrays)
        {
            return string.Join(keyword, arrays);
        }
        public string implode(string keyword, List<string> arrays)
        {
            return string.Join<string>(keyword, arrays);
        }
        public string implode(string keyword, Dictionary<int, string> arrays)
        {
            string[] tmp = new String[arrays.Keys.Count];
            int i = 0;
            foreach (int k in arrays.Keys)
            {
                tmp[i++] = arrays[k];
            }
            return string.Join(keyword, tmp);
        }
        public string implode(string keyword, Dictionary<string, string> arrays)
        {
            string[] tmp = new String[arrays.Keys.Count];
            int i = 0;
            foreach (string k in arrays.Keys)
            {
                tmp[i++] = arrays[k];
            }
            return string.Join(keyword, tmp);
        }
        public string implode(string keyword, ArrayList arrays)
        {
            string[] tmp = new String[arrays.Count];
            for (int i = 0; i < arrays.Count; i++)
            {
                tmp[i] = arrays[i].ToString();
            }
            return string.Join(keyword, tmp);
        }
        public string basename(string path)
        {
            return Path.GetFileName(path);
        }
        public string mainname(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        public string subname(string path)
        {
            //return Path.GetExtension(path);
            return Path.GetExtension(path).TrimStart('.');
        }
        public bool is_file(string filepath)
        {
            return File.Exists(filepath);
        }
        public string basedir()
        {
            //取得專案的起始位置
            return System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
        }
        public string baseurl()
        {
            // 回傳專案的網址
            return System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath;
        }
        public long filesize(string path)
        {
            FileInfo f = new FileInfo(path);
            return f.Length;
        }
        public void exit()
        {
            try
            {
                System.Web.HttpContext.Current.Response.Flush(); //強制輸出緩衝區資料
                System.Web.HttpContext.Current.Response.Clear(); //清除緩衝區的資料
                System.Web.HttpContext.Current.Response.End(); //結束資料輸出
                                                               //System.Web.HttpContext.Current.Response.StatusCode = 200;
            }
            catch
            {

            }
        }
        public byte[] file_get_contents(string url)
        {
            if (url.ToLower().IndexOf("http:") == 0 || url.ToLower().IndexOf("https:") == 0)
            {
                // URL                 
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                byte[] byteData = null;

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 60000;
                request.Proxy = null;
                request.UserAgent = _userAgent; // "user_agent','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.93 Safari/537.36";
                //request.Referer = getSystemKey("HTTP_REFERER");
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                byteData = ReadStream(stream, 32765);
                response.Close();
                stream.Close();
                return byteData;
            }
            else
            {
                byte[] data;
                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    data = ReadStream(fs, 8192);
                    fs.Close();
                };
                return data;
            }
        }
        private byte[] ReadStream(Stream stream, int initialLength)
        {
            if (initialLength < 1)
            {
                initialLength = 32768;
            }
            byte[] buffer = new byte[initialLength];
            int read = 0;
            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte == -1)
                    {
                        return buffer;
                    }
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            byte[] bytes = new byte[read];
            Array.Copy(buffer, bytes, read);
            return bytes;
        }
        public string b2s(byte[] input)
        {
            return System.Text.Encoding.UTF8.GetString(input);
        }
        public byte[] s2b(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }
        public void echoBinary(string value)
        {
            HttpContext.Current.Response.BinaryWrite(s2b(value));
        }
        public void echoBinary(byte[] value)
        {
            HttpContext.Current.Response.BinaryWrite(value);
        }
        public byte[] deflate(byte[] input)
        {
            MemoryStream ms = new MemoryStream();
            System.IO.Compression.DeflateStream ds = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress);
            ds.Write(input, 0, input.Length);
            ds.Close();
            return ms.ToArray();
        }
        // 使用 Deflate 解壓縮資料
        public byte[] decompressDeflateContent(byte[] compressedData)
        {
            using (MemoryStream memoryStream = new MemoryStream(compressedData))
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            using (MemoryStream outputMemoryStream = new MemoryStream())
            {
                // 將解壓縮資料寫入 outputMemoryStream
                deflateStream.CopyTo(outputMemoryStream);

                // 將解壓縮後的資料轉換為字串
                return outputMemoryStream.ToArray();
            }
        }
        // 解壓縮 GZIP 資料
        public byte[] decompressContent(byte[] compressedData)
        {
            using (MemoryStream memoryStream = new MemoryStream(compressedData))
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (MemoryStream outputMemoryStream = new MemoryStream())
            {
                // 將解壓縮的資料寫入到 outputMemoryStream
                gzipStream.CopyTo(outputMemoryStream);

                // 將解壓縮後的資料轉換為字串
                return outputMemoryStream.ToArray();
            }
        }

        public byte[] compressContent(byte[] data)
        {
            // 創建 MemoryStream 來壓縮資料
            // 使用 GZipStream 壓縮並傳送資料
            // 使用 MemoryStream 作為壓縮的輸出流
            using (MemoryStream compressedStream = new MemoryStream())
            {
                // 使用 GZipStream 來進行壓縮，並將結果寫入到 compressedStream 中
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal, true))
                {
                    // 把資料寫入 GZipStream，進行壓縮
                    gzipStream.Write(data, 0, data.Length);
                }

                // 返回壓縮後的 byte[]
                return compressedStream.ToArray();
            }

        }
        public string base64_encode(string data)
        {
            //base64編碼
            return Convert.ToBase64String(s2b(data));

        }
        public string base64_encode(byte[] data)
        {
            //base64編碼
            return Convert.ToBase64String(data);

        }
        public byte[] base64_decode(string data)
        {
            //base64解碼

            return Convert.FromBase64String(data);
        }
        // GZIP 壓縮方法
        public byte[] Compress(byte[] data)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            using (System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionLevel.Optimal, true))
            {
                gzipStream.Write(data, 0, data.Length);
                return compressedStream.ToArray();
            }
        }
        public DataTable sqlite_selectSQL_SAFE(string filename, string sql, Dictionary<string, object> pa)
        {
            DataTable dt = new DataTable();

            // 連接到 SQLite 資料庫
            using (var conn = new SQLiteConnection("Data Source=" + filename))
            {
                conn.Open();

                // 創建 SQLite 命令並設定 SQL 語句與參數
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    // 將參數加入到命令中
                    foreach (var param in pa)
                    {
                        cmd.Parameters.AddWithValue("@" + param.Key, param.Value);
                    }

                    // 使用 SQLiteDataReader 來執行查詢
                    using (var reader = cmd.ExecuteReader())
                    {
                        // 將結果加載到 DataTable 中

                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
        public byte[] ExtractZipToBytes(string zipFilePath, string fileNameInZip)
        {
            try
            {
                using (ZipFile zip = ZipFile.Read(zipFilePath))
                {
                    // 找到指定的檔案條目
                    ZipEntry entry = zip[fileNameInZip];

                    // 檢查該條目是否存在
                    if (entry != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // 將檔案內容從 ZipEntry 複製到 MemoryStream
                            entry.Extract(ms);

                            // 將 MemoryStream 的內容轉換為 byte[]
                            return ms.ToArray();
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found in ZIP: " + fileNameInZip);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting ZIP file: " + ex.Message);
                return null;
            }
        }
    }
}