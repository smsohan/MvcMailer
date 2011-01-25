MvcMailer is an ASP.NET MVC Mailer, inspired by Ruby on Rails ActionMailer, that helps composing Email
body using regular ASP.NET MVC views. This one works with .Net 4.0 for now.


How to Use
----------------

	Install-Package MvcMailer
	
This will download from the [official feed](http://go.microsoft.com/fwlink/?LinkID=206669)

Build From Source
-------------------	
* Download the source code
* Build the solution, you will need to have Nuget installed
* Once Build Succeeds, you will see the the package inside Mvc.Mailer\NuGet\Output 


How Does it Work?
-------------------
When MvcMailer NuGet is installed, it does the following:

* Adds a reference to MvcMailer.dll
* Adds mailSettings configSection to web.config file
* Adds a sample Mailer called Notifier inside Mailers/Notifier.cs
* Adds two views: WelcomeMessage.cshml and _Layout.cshtml inside Views/Notifier
* Adds Mvc.Mailer to namespaces of the Razor view pages so that the helper methods and extensions are available to views without a using
* To send and email, take a look inside the Mailers/Notifier.cs file. It looks like the following:

The Minimal Version
--------------------------------
Installing the package will install a class called Notifier inside Mailers. Just follow the simple 2 line code example to send your email.
This is how it looks:

	public MailMessage WelcomeMessage()
	{
		var mailMessage = new MailMessage { Subject = "Testing by Sohan" };
		ViewBag.Name = "Jon Doe";
		mailMessage.Body = PopulateBody(mailMessage, "WelcomeMessage");
		mailMessage.To.Add("some-email@gmail.com");
		return mailMessage;
	}

In your controller or moder or elsewhere, use the following to send the email:

	using Mvc.Mailer;
	using MyApp.Mailers;

	new Notifier().WelcomeMessage().Send();

In Your View
-----------------
Views/Notifier/WelcomeMessage.cshtml

	Hi @ViewBag.Name: <br />
	Welcome to my site!

	Thank you.


Views/Notifier/_Layout.cshtml
	
	<html>
		<head></head>
		<body>
			<div class="banner">
				
			</div>

			@RenderBody();


			<br />
			--
			The site team!<br/>
			xxx-xxx-xxxx

			<div class="footer">
			</div>
		</body>
	</html>


Web.config
------------
Find mailSettings inside web.config and edit the dummy paramters to real ones!


Absolute URL in Emails
-----------------------
Unlike your MVC views, the Email views need to show absolute URL. This can be achieved be using the following syntax:


	@Url.Abs(Url.Action("About", "Home"))

Abs is an extension method on top of UrlHelper and it converts your relative Urls to Absolute urls, so that the email receipients
can follow the links!


Learn More
------------
Read my [CodeProject article](http://www.codeproject.com/KB/aspnet/MvcMailerNuGet.aspx)

Look inside the Notifier class to tune the parameters. To learn more, feel free to see the code comments on the MailerBase and Notifier classes.
	


Sending Asynchronous Email
---------------------------
To send asynchronous/non blocking emails, it's easy to use the SendAsync extension method on MailMessage.

	new Notifier().WelcomeMessage().SendAsync();
	
Or if you need to handle AsyncCompleted/Error events, 
just use the [SMTPClient.SendAsync method](http://msdn.microsoft.com/en-us/library/x5x13z6h.aspx)

Hope this is fun for your to use MvcMailer. In case of a feedback or trouble, please feel free to contact at @smsohan