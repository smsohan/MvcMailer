MvcMailer is an ASP.NET MVC Mailer, inspired by Ruby on Rails ActionMailer, that helps composing Email
body using regular ASP.NET MVC views. This one works with .Net 4.0 for now.


##How to Use

See [MvcMailer Wiki](https://github.com/smsohan/MvcMailer/wiki/MvcMailer-Step-by-Step-Guide)

##Visual Studio 2103 NOTE

 Scaffolding in Visual Studio 2013 is changing (and has changed) and won't be supported forever in its current form. However, there is a -preelease vs2013 version of the T4Scaffolder and if you want to use MvcMailer scaffolding with VS2013, then you'll need the right package version! 

* Good news - This all work fine under VS3013 when the it's run _as Admin_.
* Bad news - Because this NuGet will have a dependency on a -pre (prelease) package, then it becomes prerelease per http://docs.nuget.org/docs/reference/versioning#Prerelease_Versions

That means, you would **install the VS2013 version of MvcMailer** like this:

```
install-package MvcMailer-vs2013 -pre
```

Which in turn brings in the T4 -pre dependency. You can also select "include prerelease" when searching within the Visual Studio NuGet GUI.


##Report Feature or Issues

Use [Github Issues Page](https://github.com/smsohan/mvcmailer/issues) to mention your desired feature or if something is broken!

##Have Fun

I have a lot of fun working on this project. Wish you enjoy using it and if you wish, you can contribute too. Find me on Twitter [@smsohan](http://twitter.com/smsohan)

## Other Contributors
[@TylerMercier](https://github.com/tylermercier)