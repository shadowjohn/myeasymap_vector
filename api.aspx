<%@ Page Language="C#" AutoEventWireup="true" CodeFile="api.aspx.cs" Inherits="api" %>

<%@ Import Namespace="utility" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.IO" %>
<%
    myinclude my = new myinclude();
    string GETS_STRING = "mode";
    var GETS = my.getGET_POST(GETS_STRING, "GET");
    switch (GETS["mode"].ToString())
    {
        case "getStyle":
            {
                GETS_STRING = "osmData";
                GETS = my.getGET_POST(GETS_STRING, "GET");
                string osmName = my.mainname(GETS["osmData"].ToString());
                string osmDataFile = Path.Combine(my.basedir(), "data", osmName, osmName) + ".style";
                if (my.is_file(osmDataFile))
                {
                    // 載入檔案內容，要取代一些字串
                    string styleSTR = my.b2s(my.file_get_contents(osmDataFile));
                    styleSTR = styleSTR.Replace("{{API_URL}}", my.baseurl() + "/api.aspx");
                    styleSTR = styleSTR.Replace("{{BASE_URL}}", my.baseurl());
                    styleSTR = styleSTR.Replace("{{osmName}}", osmName);

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Headers.Add("Content-Type", "application/json");
                    //HttpContext.Current.Response.Headers.Add("Content-Length", styleSTR.Length.ToString());
                    //HttpContext.Current.Response.TransmitFile(osmDataFile);
                    my.echoBinary(styleSTR);
                }
                my.exit();
            }
            break;
        case "getV3":
            {
                GETS_STRING = "osmData";
                GETS = my.getGET_POST(GETS_STRING, "GET");
                string osmName = my.mainname(GETS["osmData"].ToString());
                string osmDataFile = Path.Combine(my.basedir(), "data", osmName, osmName) + ".v3";

                if (my.is_file(osmDataFile))
                {
                    // 載入檔案內容，要取代一些字串
                    string styleSTR = my.b2s(my.file_get_contents(osmDataFile));
                    styleSTR = styleSTR.Replace("{{API_URL}}", my.baseurl() + "/api.aspx");
                    styleSTR = styleSTR.Replace("{{BASE_URL}}", my.baseurl());
                    styleSTR = styleSTR.Replace("{{osmName}}", osmName);

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Headers.Add("Content-Type", "application/json");
                    //HttpContext.Current.Response.Headers.Add("Content-Length", styleSTR.Length.ToString());
                    //HttpContext.Current.Response.TransmitFile(osmDataFile);
                    my.echoBinary(styleSTR);
                }
                my.exit();
            }
            break;
        case "getMap":
            {
                // 改成從 zip 讀
                GETS_STRING = "osmData,x,y,z";
                GETS = my.getGET_POST(GETS_STRING, "GET");
                int x = Convert.ToInt32(GETS["x"].ToString());
                int y = Convert.ToInt32(GETS["y"].ToString());
                int z = Convert.ToInt32(GETS["z"].ToString());
                y = ((1 << z) - y - 1);
                string osmName = my.mainname(GETS["osmData"].ToString());
                string osmDataFile = Path.Combine(my.basedir(), "data", osmName, osmName) + ".zip";
                if (!my.is_file(osmDataFile))
                {
                    my.exit();
                }
                // 從 zip 讀出 pbf
                string pbfFile = "tiles/" + z + "/" + x + "/" + y + ".pbf";
                // zip 是否有檔
                byte[] b = my.ExtractZipToBytes(osmDataFile, pbfFile);
                if (b != null)
                {
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Headers.Add("Content-Transfer-Encoding", "binary");
                    HttpContext.Current.Response.Headers.Add("Content-Type", "application/x-protobuf");
                    HttpContext.Current.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + z + "_" + x + "_" + y + ".pbf\"");
                    my.echoBinary(b);
                }

                my.exit();
            }
            break;        
    }
%>