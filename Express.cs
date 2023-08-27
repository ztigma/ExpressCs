using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Web;

namespace Express
{
	public class App
	{
		static Encoding enc = Encoding.UTF8;
		TcpListener listener;
		TcpClient client;
		NetworkStream stream;
		Request req;
		Response res;
		public List<ResEvent> ResEvents = new List<ResEvent>();
		
		public App()
		{
			Console.WriteLine ("Server Stand by...");
			listener = new TcpListener (IPAddress.Any, 8080);
			listener.Start ();
			Console.WriteLine ("Server Start!");
		}
		public void Listen()
		{
			Console.WriteLine ("Server is Listen!!!");
			while(true)
			{
				client = listener.AcceptTcpClient();
				stream = client.GetStream ();
				
				Console.WriteLine("REQUEST INCOMING:");
				//Console.WriteLine(stream.TO_STRING());
				
				req = stream.REQUEST();
				res = new Response(stream, client);
	
				Console.WriteLine(req.method);
				Console.WriteLine(req.path);
				
				var find = ResEvents.Find(n => n.method.ToLower() == req.method.ToLower() && n.path.ToLower() == req.path.ToLower());
	
				if(find == null)
				{
					Console.WriteLine("NONE RESPONSE IT'S PREPARED (WARNING)");
					res.send("CANNOT : " + req.method + " : " + req.path);
				}
				else
				{
					if(find.method == "GET")
					{
						Console.WriteLine("GET Response deploying");
						find.req_res(req, res);
					}
					else
					{
						Console.WriteLine("POST Response deploying");
						find.req_res(req, res);
					}
				}
			}
		}
		public void get(string path, Req_Res req_res)
		{
			Console.WriteLine ("GET ADD! : " + path);
			ResEvents.Add(new ResEvent("GET", path, req_res));
		}
		public void post(string path, Req_Res req_res)
		{
			ResEvents.Add(new ResEvent("POST", path, req_res));
		}
	}
	public static class Extensions
	{
		public static Encoding enc = Encoding.UTF8;

		public static string TO_STRING(this List<NV> body)
		{
			if(body == null)
			{
				return "undefined";
			}
			
			string r = "";
			body.ForEach(n => r += $"{n.name} = {n.value} \n");
			return r;
		}
		public static Request REQUEST(this NetworkStream stream)
		{
			return new Request(stream); 
		}
		public static List<string> req_list(this NetworkStream stream)
		{
			return new List<string>(stream.TO_STRING().Split("\n"));
		}
		public static string req_string(this NetworkStream stream)
		{
			return stream.TO_STRING();	
		}
		public static string TO_STRING(this NetworkStream stream)
		{
			MemoryStream memoryStream = new MemoryStream ();
			byte[] data = new byte[256];
			int size;
			do {
				size = stream.Read (data, 0, data.Length);
				if (size == 0) {
					Console.WriteLine ("client disconnected...");
					Console.ReadLine ();
					return  null; 
				} 
				memoryStream.Write (data, 0, size);
			} while ( stream.DataAvailable); 
			return enc.GetString (memoryStream.ToArray ());
		}
	}
	public class Request
	{
		public string method;
		public string path;
		public string path_raw;
		public string httpVersion;
		public string raw;
		
		public List<NV> body;
	
		public Request(NetworkStream stream)
		{
			raw = stream.TO_STRING();
			var split = raw.Split("\n")[0].Split(" ");
			method = split[0];
			path_raw = split[1];
			path = path_raw.Split("?")[0];
			httpVersion = split[2];
	
			if(method == "GET")
			{
				var data = path_raw.Split("?");
	
				if(data.Length <= 1)
				{
					return;
				}
		
				try
				{
					string body_raw = data[1];
					var body_data = body_raw.Split("&");
					List<string> data_raw = new List<string>(body_data);
					body = data_raw.ConvertAll
					(
						(n) =>
						{
							var spl = n.Split("=");
							return new NV
							(
								HttpUtility.UrlDecode(spl[0])
								, 
								HttpUtility.UrlDecode(spl[1])
							);
						}
					);
				}
				catch(Exception ex)
				{
					body = null;
					Console.WriteLine(ex);
				}	
			}
			else if(method == "POST")
			{
				var split_data = raw.Split("\n");
				var data = split_data[split_data.Length - 1];
				//Console.WriteLine($"POST DATA: {data}");

				try
				{
					string body_raw = data;
					var body_data = body_raw.Split("&");
					List<string> data_raw = new List<string>(body_data);
					body = data_raw.ConvertAll
					(
						(n) =>
						{
							var spl = n.Split("=");
							return new NV
							(
								HttpUtility.UrlDecode(spl[0])
								, 
								HttpUtility.UrlDecode(spl[1])
							);
						}
					);
				}
				catch(Exception ex)
				{
					body = null;
					Console.WriteLine(ex);
				}
			}
		}
	}
	public class Response
	{
		TcpClient client;
		NetworkStream stream;
	
		public Response(NetworkStream STREAM, TcpClient CLIENT)
		{
			stream = STREAM;
			client = CLIENT;
		}
		public void send(string data)
		{
			StringBuilder builder = new StringBuilder ();
			builder.AppendLine (@"HTTP/1.1 200 OK"); 
			builder.AppendLine (@"Content-Type: text/html");
			builder.AppendLine (@"");
			builder.AppendLine (data);
			
			byte[] sendBytes = Extensions.enc.GetBytes (builder.ToString ());
			stream.Write (sendBytes, 0, sendBytes.Length);
	
			stream.Close ();
			client.Close ();
		}
		public void end(byte[] sendBytes)
		{
			stream.Write (sendBytes, 0, sendBytes.Length);
	
			stream.Close ();
			client.Close ();
		}
	}
	public delegate void Req_Res(Request req, Response res);
	
	public class ResEvent
	{
		public string method;
		public string path;
		public Req_Res req_res;
	
		public ResEvent(string METHOD, string PATH, Req_Res REQ_RES)
		{
			method = METHOD;
			path = PATH;
			req_res = REQ_RES;
		}
	}
	public class NV
	{
		public string name;
		public string value;
	
		public NV(string NAME, string VALUE)
		{
			name = NAME;
			value = VALUE;
		}
		public override string ToString()
		{
			return value;
		}
	}
}