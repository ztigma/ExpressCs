using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Express;
using ViewHtml;

class Program
{ 
	public static void Main (string[] args)
	{
		var app = new App();
		
		app.get
		(
			"/"
			, 
			(Request req, Response res) =>
			{
				Console.WriteLine("Respondiendo a Home!");
				Console.WriteLine("BODY:");
				Console.WriteLine(req.body.TO_STRING());
				
				res.send
				(
					new View()
					.Tag("vista")
					.Attributes
					(
						new NV[]
						{
							new NV("_orientation", "vertical_center")
						}
					)
					.Children
					(
						new View[]
						{
							new View()
							.Style
							(
								new NV[]
								{
									new NV("color", "blue")
								}
							)
							.Children("jaja")
							,
							new View()
							.Style
							(
								new NV[]
								{
									new NV("color", "orange")
								}
							)
							.Children("jeje")
							,
							new View()
							.Tag("a")
							.Attributes
							(
								new NV[]
								{
									new NV("href", "/form")
								}
							)
							.Children("Ir al formulario")
								
						}
					)
				);
			}
		);

		app.get
		(
			"/form"
			,
			(Request req, Response res) =>
			{
				res.send
				(
					new View()
					.Tag("form")
					.Attributes
					(
						new NV[]
						{
							new NV("method", "post")
							,
							new NV("action", "/form")
						}
					)
					.Children
					(
						new View[]
						{
							new View()
							.Tag("input")
							.Attributes
							(
								new NV[]
								{
									new NV("placeholder", "escriba aqui...")
									,
									new NV("name", "escribir")
								}
							)
							,
							new View()
							.Tag("input")
							.Attributes
							(
								new NV[]
								{
									new NV("placeholder", "escriba aqui...")
									,
									new NV("name", "otro")
								}
							)
							,
							new View()
							.Tag("button")
							.Children("Enviar Form")
						}
					)
				);
			}
		);

		app.post
		(
			"/form"
			,
			(Request req, Response res) =>
			{
				Console.WriteLine("BODY:");
				Console.WriteLine(req.body.TO_STRING());
				
				res.send
				(
					new View()
					.Tag("a")
					.Attributes
					(
						new NV[]
						{
							new NV("href", "/form")
						}
					)
					.Children("Quiere volver al formulario? presione aqui")
				);
			}
		);
		
		app.Listen();
	}
}