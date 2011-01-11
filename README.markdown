MvcMailer is an ASP.NET MVC Mailer, inspired by Ruby on Rails ActionMailer, that helps composing Email
body using regular ASP.NET MVC views. This one works with .Net 4.0 for now.


How to Use
----------------
* Download the NuGet Package to a Folder e.g. C:\\Nuggets 
	https://github.com/smsohan/MvcMailer/blob/master/Mvc.Mailer/NuGet/output/MvcMailer.0.1.nupkg
* Add this location to Package Manager
* Install MvcMailer NuGet using the following:

	Install-Package MvcMailer
	

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
* To send and email, take a look inside the Mailers/Notifier.cs file. It looks like the following:


	namespace MvcApplication1.Mailers
	{
		/// <summary>
		/// To Send a welcome message do the following:
		/// using Mvc.Mailer;
		/// new Notifier().WelcomeMessage().Send();
		/// The web.config file section for mails is already added to your project.
		/// Just edit your web.config file mailSettings and provide with server, port, user, password etc.
		/// </summary>
		public class Notifier : MailerBase
		{
			/// <summary>
			/// In your constructor you can specify a default MasterName
			/// or a Default mime type to Html or Text
			/// </summary>
			public Notifier()
			{
				this.MasterName = "~/Views/Notifier/_Layout.cshtml";
				this.IsBodyHtml = true;
			}

			/// <summary>
			/// Gnereate a welcome message
			/// </summary>
			/// <returns></returns>
			public MailMessage WelcomeMessage()
			{
				//Create a MailMessage object
				var mailMessage = new MailMessage { Subject = "Testing by Sohan" };

				//cretae an instance of viewData if you need StronglyTyped views or any ViewData
				//var viewData = new ViewDataDictionary<Address>
				//{
				//    Model = new Address { City = "Calgary", Province = "AB", Street = "600 6th Ave NW" }
				//};
				
				//Get the EmailBody - this will be the string containing the view after it has been rendered by your ViewEngine
				mailMessage.Body = PopulateBody(mailMessage: mailMessage, 
												viewName: "~/Views/Notifier/WelcomeMessage.cshtml" 
												//,viewData: viewData
												// Uncomment the following to Customize the masterName for this message
												//,masterName: "~/Views/Mailer/_AnotherLayout.cshtml"
												);
				
				//Uncomment the following if you want to send a Text/Plain email instead
				//mailMessage.IsBodyHtml = false;

				//Add one/more Recipient(s)
				mailMessage.To.Add("some-email@gmail.com");

				//Edit the following line to Add an Attachment
				//mailMessage.Attachments.Add(new Attachment("SitePolicy.pdf"));
				
				return mailMessage;
			}
		}
	}
	
	
Now, with this class already installed, all you need to do is the following to send your email:

	// This one adds the Send() Extension method to MailMessage class
	using Mvc.Mailer;
	...
	new Notifier().WelcomeMessage().Send();
	
Of course, you want to set the network configuration at web.config mailSettings. Its already there, just set
it up with the right values.

Hope this is fun for your to use MvcMailer. In case of a feedback or trouble, please feel free to contact at @smsohan