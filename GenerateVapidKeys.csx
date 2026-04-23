#!/usr/bin/env dotnet-script
#r "nuget: WebPush, 1.0.12"

using WebPush;

// VAPID-Keys generieren
var vapidKeys = VapidHelper.GenerateVapidKeys();

Console.WriteLine("========================================");
Console.WriteLine("VAPID Keys für Push-Benachrichtigungen");
Console.WriteLine("========================================");
Console.WriteLine();
Console.WriteLine("Public Key:");
Console.WriteLine(vapidKeys.PublicKey);
Console.WriteLine();
Console.WriteLine("Private Key:");
Console.WriteLine(vapidKeys.PrivateKey);
Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine("Füge diese Keys in appsettings.json ein:");
Console.WriteLine("========================================");
Console.WriteLine();
Console.WriteLine("\"PushNotification\": {");
Console.WriteLine($"  \"PublicKey\": \"{vapidKeys.PublicKey}\",");
Console.WriteLine($"  \"PrivateKey\": \"{vapidKeys.PrivateKey}\",");
Console.WriteLine("  \"Subject\": \"mailto:deine-email@beispiel.de\"");
Console.WriteLine("}");
Console.WriteLine();
