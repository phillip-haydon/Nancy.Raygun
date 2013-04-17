Nancy.Raygun
============

Raygun is a service for automatically reporting, tracking and alerting you to errors in your applications. Discover and resolve errors faster than ever before and keep your users happy.

[http://raygun.io/](http://raygun.io/?ref=1QWEy)

This library is for use with a <www.nancyfx.org> application, it's based on the offical library <https://github.com/MindscapeHQ/raygun4net> from Mindscape, with the key different that it uses the `NancyContext` rather than `HttpContext.Current`, meaning more information can be included specific to a Nancy application. It also wires up the error handling for you just by including the project into your application, by implementing `IApplicationStartup` for you.

## Installing Nancy.Raygun

Using Nuget, install the Nancy.Raygun package.

> PM> Install-Package Nancy.Raygun

## Configuring Raygun (RaygunSettings)
In your web.config add the section

    <configSections>
      <section name="RaygunSettings" type="Nancy.Raygun.RaygunSettings, Nancy.Raygun" />
    </configSections>
    
And the setting for your API Key

    <RaygunSettings apikey="* your api key goes here *" />

## Configuring Raygun (appSettings)
If you're hosting on Azure/AppHabor then you may want to use appSettings instead so you can add the key via the portal/administration area.

To do this, add a the key `nr.apiKey` to your appSettings, with the value being your API Key.

	<appSettings>
	  <add key="nr.apiKey" value="* your api key goes here *" />
	</appSettings>

----------

That's it, now you can enjoy watching all your errors get Zapped by Raygun. Don't let Robby down, he wants your errors!