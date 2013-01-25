Nancy.Raygun
============

Raygun is a service for automatically reporting, tracking and alerting you to errors in your applications. Discover and resolve errors faster than ever before and keep your users happy.

[http://raygun.io/](http://raygun.io/?ref=1QWEy)

This library is for use with a <www.nancyfx.org> application, it's based on the offical library <https://github.com/MindscapeHQ/raygun4net> from Mindscape, with the key different that it uses the `NancyContext` rather than `HttpContext.Current`, meaning more information can be included specific to a Nancy application. It also wires up the error handling for you just by including the project into your application, by implementing `IApplicationStartup` for you.

To use Nancy.Raygun, install the nuget package

> PM> Install-Package Nancy.Raygun

Then in your web.config add the section

    <configSections>
      <section name="RaygunSettings" type="Nancy.Raygun.RaygunSettings, Nancy.Raygun" />
    </configSections>
    
And the setting for your API Key

    <RaygunSettings apikey=" your api key goes here " />
    
And that's it, now you can enjoy watching all your errors get Zapped by Raygun. Don't let Robby down, he wants your errors!