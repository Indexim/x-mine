﻿using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace X_MINE.Controllers
{
	[Route("el-finder/file-system")]
	public class FileSystemController : Controller
	{
		[Route("connector")]
		public async Task<IActionResult> Connector()
		{
			try
			{
				var connector = GetConnector();
				
				/*Console.WriteLine($"connector: {JsonConvert.SerializeObject(connector)}");*/
				var result =  await connector.ProcessAsync(Request);
				/*Console.WriteLine(result != null ? result.ToJson()[0] : "");*/
				var jsonString = JsonConvert.SerializeObject(result);
				Console.WriteLine($"result: {jsonString}");

				return result;
			}
			catch (Exception ex)
			{
				// TODO: would be good to Sanitize Exception Message at least in production, can leak server file paths
				return Json(new { error = "Unable to process your request: " + ex.Message });
			}
		}

		[Route("thumb/{hash}")]
		public async Task<IActionResult> Thumbs(string hash)
		{
			try
			{
				var connector = GetConnector();
				return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
			}
			catch (Exception ex)
			{
				// TODO: would be good to Sanitize Exception Message at least in production, can leak server file paths
				return Json(new { error = "Unable to process your request: " + ex.Message });
			}
		}

		private Connector GetConnector()
		{
			var driver = new FileSystemDriver();

			string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
			var uri = new Uri(absoluteUrl);

			var root = new RootVolume(
				PathHelper.MapPath("~/Files"),
				$"{uri.Scheme}://{uri.Authority}/Files/",
				$"{uri.Scheme}://{uri.Authority}/el-finder/file-system/thumb/")
				{
					//IsReadOnly = !User.IsInRole("Administrators")
					IsReadOnly = false, // Can be readonly according to user's membership permission
					IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
					Alias = "Files", // Beautiful name given to the root/home folder
									 //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
					AccessControlAttributes = new HashSet<NamedAccessControlAttributeSet>()
				{
					new NamedAccessControlAttributeSet(PathHelper.MapPath("~/Files/readonly.txt"))
					{
						Write = false,
						Locked = true
					},
					new NamedAccessControlAttributeSet(PathHelper.MapPath("~/Files/Prohibited"))
					{
						Read = false,
						Write = false,
						Locked = true
					},
					new NamedAccessControlAttributeSet(PathHelper.MapPath("~/Files/Parent/Children"))
					{
						Read = true,
						Write = false,
						Locked = true
					}
				},
					// Upload file type constraints
					//UploadAllow = new[] { "image", "text" },
					//UploadDeny = new[] { "text/csv" },
					//UploadOrder = new[] { "allow", "deny" }
				};

			driver.AddRoot(root);
			/*Console.WriteLine($"root: {root.ToJson()}");
			Console.WriteLine($"driver: {driver.Roots.ToList()}");*/
			return new Connector(driver)
			{
				// This allows support for the "onlyMimes" option on the client.
				MimeDetect = MimeDetectOption.Internal
			};
		}
	}
}