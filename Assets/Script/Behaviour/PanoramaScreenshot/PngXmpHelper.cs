using System;

namespace Assets.Script.Behaviour.PanormaScreenshot
{
		public static class PngXmpHelper
		{
				private const string XMP_CONTENT_TO_FORMAT_PNG = "XML:com.adobe.xmp\0\0\0\0\0<?xpacket begin=\"ï»¿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?><x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 5.1.0-jc003\"> <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"> <rdf:Description rdf:about=\"\" xmlns:GPano=\"http://ns.google.com/photos/1.0/panorama/\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\" xmlns:stEvt=\"http://ns.adobe.com/xap/1.0/sType/ResourceEvent#\" xmlns:tiff=\"http://ns.adobe.com/tiff/1.0/\" xmlns:exif=\"http://ns.adobe.com/exif/1.0/\"> <GPano:UsePanoramaViewer>True</GPano:UsePanoramaViewer> <GPano:CaptureSoftware>Unity3D</GPano:CaptureSoftware> <GPano:StitchingSoftware>Unity3D</GPano:StitchingSoftware> <GPano:ProjectionType>equirectangular</GPano:ProjectionType> <GPano:PoseHeadingDegrees>180.0</GPano:PoseHeadingDegrees> <GPano:InitialViewHeadingDegrees>0.0</GPano:InitialViewHeadingDegrees> <GPano:InitialViewPitchDegrees>0.0</GPano:InitialViewPitchDegrees> <GPano:InitialViewRollDegrees>0.0</GPano:InitialViewRollDegrees> <GPano:InitialHorizontalFOVDegrees>{0}</GPano:InitialHorizontalFOVDegrees> <GPano:CroppedAreaLeftPixels>0</GPano:CroppedAreaLeftPixels> <GPano:CroppedAreaTopPixels>0</GPano:CroppedAreaTopPixels> <GPano:CroppedAreaImageWidthPixels>{1}</GPano:CroppedAreaImageWidthPixels> <GPano:CroppedAreaImageHeightPixels>{2}</GPano:CroppedAreaImageHeightPixels> <GPano:FullPanoWidthPixels>{1}</GPano:FullPanoWidthPixels> <GPano:FullPanoHeightPixels>{2}</GPano:FullPanoHeightPixels> <tiff:Orientation>1</tiff:Orientation> <exif:PixelXDimension>{1}</exif:PixelXDimension> <exif:PixelYDimension>{2}</exif:PixelYDimension> </rdf:Description></rdf:RDF></x:xmpmeta><?xpacket end=\"w\"?>";
				private static uint[] CRC_TABLE_PNG = null;

				public static byte[] EnableXmp360ImageViewer(byte[] fileBytes, int imageWidth, int imageHeight)
				{
						string xmpContent = "iTXt" + string.Format(XMP_CONTENT_TO_FORMAT_PNG, 75f.ToString("F1"), imageWidth, imageHeight);
						int copyBytesUntil = 33;
						int xmpLength = xmpContent.Length - 4; // minus iTXt
						string xmpCRC = CalculateCRC_PNG(xmpContent);
						xmpContent = string.Concat((char)(xmpLength >> 24), (char)(xmpLength >> 16), (char)(xmpLength >> 8), (char)(xmpLength),
													xmpContent, xmpCRC);

						byte[] result = new byte[fileBytes.Length + xmpContent.Length];

						Array.Copy(fileBytes, 0, result, 0, copyBytesUntil);

						for (int i = 0; i < xmpContent.Length; i++)
						{
								result[copyBytesUntil + i] = (byte)xmpContent[i];
						}

						Array.Copy(fileBytes, copyBytesUntil, result, copyBytesUntil + xmpContent.Length, fileBytes.Length - copyBytesUntil);

						return result;
				}

				// Source: https://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc32.cs
				private static string CalculateCRC_PNG(string xmpContent)
				{
						if (CRC_TABLE_PNG == null)
								CalculateCRCTable_PNG();

						uint crc = ~UpdateCRC_PNG(xmpContent);
						byte[] crcBytes = CalculateCRCBytes_PNG(crc);

						return string.Concat((char)crcBytes[0], (char)crcBytes[1], (char)crcBytes[2], (char)crcBytes[3]);
				}

				private static uint UpdateCRC_PNG(string xmpContent)
				{
						uint c = 0xFFFFFFFF;
						for (int i = 0; i < xmpContent.Length; i++)
						{
								c = (c >> 8) ^ CRC_TABLE_PNG[xmpContent[i] ^ c & 0xFF];
						}

						return c;
				}

				private static void CalculateCRCTable_PNG()
				{
						CRC_TABLE_PNG = new uint[256];
						for (uint i = 0; i < 256; i++)
						{
								uint c = i;
								for (int j = 0; j < 8; j++)
								{
										if ((c & 1) == 1)
												c = (c >> 1) ^ 0xEDB88320;
										else
												c = (c >> 1);
								}

								CRC_TABLE_PNG[i] = c;
						}
				}

				private static byte[] CalculateCRCBytes_PNG(uint crc)
				{
						var result = BitConverter.GetBytes(crc);

						if (BitConverter.IsLittleEndian)
								Array.Reverse(result);

						return result;
				}
		}
}