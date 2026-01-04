// Data/WeatherDataSeeder.cs
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public static class WeatherDataSeeder
    {
        public static async Task SeedWeatherData(ApplicationDbContext context)
        {
            // Prüfen ob bereits Wetterdaten vorhanden sind
            if (await context.WeatherOptions.AnyAsync())
            {
                return; // Daten bereits vorhanden
            }

            var weatherData = new Dictionary<string, List<(string weather, List<(string day, string icon, string temp)> forecast)>>
            {
                ["Januar"] = new()
                {
                    ("Kalter, klarer Himmel", new() { ("Mo", "☀️", "2°/-5°"), ("Di", "☀️", "1°/-6°"), ("Mi", "☀️", "2°/-5°"), ("Do", "☁️", "0°/-4°"), ("Fr", "☀️", "3°/-3°") }),
                    ("Anhaltender Schneefall", new() { ("Mo", "🌨️", "-1°/-4°"), ("Di", "🌨️", "-2°/-5°"), ("Mi", "❄️", "-3°/-7°"), ("Do", "🌨️", "-2°/-5°"), ("Fr", "☁️", "0°/-3°") }),
                    ("Beißender Wind und Frost", new() { ("Mo", "💨", "-5°/-10°"), ("Di", "💨", "-6°/-12°"), ("Mi", "❄️", "-7°/-14°"), ("Do", "💨", "-6°/-11°"), ("Fr", "☀️", "-4°/-8°") })
                },
                ["Februar"] = new()
                {
                    ("Grauer Himmel, Schneeregen", new() { ("Mo", "🌧️", "1°/-2°"), ("Di", "🌧️", "0°/-3°"), ("Mi", "🌨️", "-1°/-4°"), ("Do", "🌧️", "2°/0°"), ("Fr", "🌧️", "1°/-1°") }),
                    ("Überraschender Wärmeeinbruch", new() { ("Mo", "☀️", "10°/2°"), ("Di", "☀️", "12°/3°"), ("Mi", "🌧️", "8°/1°"), ("Do", "☁️", "7°/0°"), ("Fr", "☀️", "11°/2°") }),
                    ("Eisige Nächte, sonnige Tage", new() { ("Mo", "☀️", "4°/-4°"), ("Di", "☀️", "5°/-5°"), ("Mi", "☀️", "6°/-4°"), ("Do", "☀️", "5°/-3°"), ("Fr", "☁️", "3°/-2°") })
                },
                ["März"] = new()
                {
                    ("Stürmische Winde, erster Regen", new() { ("Mo", "💨", "8°/3°"), ("Di", "🌧️", "7°/4°"), ("Mi", "🌧️", "6°/2°"), ("Do", "💨", "9°/3°"), ("Fr", "☁️", "7°/1°") }),
                    ("Langsame Schneeschmelze", new() { ("Mo", "☁️", "3°/0°"), ("Di", "🌧️", "4°/1°"), ("Mi", "🌫️", "2°/0°"), ("Do", "☀️", "5°/1°"), ("Fr", "☁️", "4°/0°") }),
                    ("Wechselhaft mit Sonne und Schauern", new() { ("Mo", "⛅", "10°/4°"), ("Di", "🌧️", "8°/3°"), ("Mi", "☀️", "12°/5°"), ("Do", "🌧️", "9°/4°"), ("Fr", "⛅", "11°/5°") })
                },
                ["April"] = new()
                {
                    ("Heftige Regenschauer", new() { ("Mo", "🌧️", "9°/5°"), ("Di", "🌧️", "8°/4°"), ("Mi", "⛈️", "10°/6°"), ("Do", "🌧️", "7°/3°"), ("Fr", "☁️", "8°/4°") }),
                    ("Milder Frühlingsbeginn", new() { ("Mo", "☀️", "14°/6°"), ("Di", "⛅", "15°/7°"), ("Mi", "☀️", "16°/8°"), ("Do", "☁️", "13°/6°"), ("Fr", "🌧️", "12°/5°") }),
                    ("Später Frosteinbruch", new() { ("Mo", "❄️", "5°/-2°"), ("Di", "❄️", "4°/-3°"), ("Mi", "☀️", "6°/-1°"), ("Do", "☁️", "5°/0°"), ("Fr", "🌧️", "3°/-1°") })
                },
                ["Mai"] = new()
                {
                    ("Warme, sonnige Tage", new() { ("Mo", "☀️", "22°/12°"), ("Di", "☀️", "24°/14°"), ("Mi", "☀️", "25°/15°"), ("Do", "☀️", "23°/13°"), ("Fr", "⛅", "21°/12°") }),
                    ("Gewitter am Nachmittag", new() { ("Mo", "⛅", "23°/14°"), ("Di", "⛈️", "20°/15°"), ("Mi", "⛅", "24°/16°"), ("Do", "⛈️", "21°/15°"), ("Fr", "☀️", "25°/17°") }),
                    ("Kühle, feuchte Morgen", new() { ("Mo", "🌫️", "18°/10°"), ("Di", "🌫️", "19°/11°"), ("Mi", "☀️", "21°/12°"), ("Do", "🌫️", "17°/9°"), ("Fr", "☁️", "18°/10°") })
                },
                ["Juni"] = new()
                {
                    ("Heiß und trocken", new() { ("Mo", "☀️", "28°/18°"), ("Di", "☀️", "30°/20°"), ("Mi", "☀️", "31°/21°"), ("Do", "☀️", "29°/19°"), ("Fr", "☀️", "30°/20°") }),
                    ("Schwüle mit häufigen Gewittern", new() { ("Mo", "⛈️", "26°/19°"), ("Di", "☁️", "27°/20°"), ("Mi", "⛈️", "25°/18°"), ("Do", "⛈️", "26°/19°"), ("Fr", "🌧️", "24°/18°") }),
                    ("Angenehm warm mit leichter Brise", new() { ("Mo", "⛅", "24°/16°"), ("Di", "💨", "23°/15°"), ("Mi", "☀️", "25°/17°"), ("Do", "💨", "22°/14°"), ("Fr", "⛅", "24°/16°") })
                },
                ["Juli"] = new()
                {
                    ("Drückende Hitze, Dürre", new() { ("Mo", "☀️", "33°/22°"), ("Di", "☀️", "35°/24°"), ("Mi", "☀️", "36°/25°"), ("Do", "☀️", "34°/23°"), ("Fr", "🔥", "37°/26°") }),
                    ("Kurze, heftige Sommergewitter", new() { ("Mo", "☀️", "30°/20°"), ("Di", "⛈️", "28°/19°"), ("Mi", "☀️", "31°/21°"), ("Do", "⛈️", "29°/20°"), ("Fr", "☀️", "32°/22°") }),
                    ("Warme Nächte unter klarem Himmel", new() { ("Mo", "🌙", "28°/19°"), ("Di", "🌙", "29°/20°"), ("Mi", "🌙", "30°/21°"), ("Do", "🌙", "28°/19°"), ("Fr", "🌙", "29°/20°") })
                },
                ["August"] = new()
                {
                    ("Erntewetter, goldenes Licht", new() { ("Mo", "☀️", "25°/15°"), ("Di", "☀️", "26°/16°"), ("Mi", "☀️", "27°/17°"), ("Do", "⛅", "24°/14°"), ("Fr", "☀️", "25°/15°") }),
                    ("Hitzewelle mit Waldbrandgefahr", new() { ("Mo", "🔥", "32°/21°"), ("Di", "🔥", "34°/23°"), ("Mi", "🔥", "35°/24°"), ("Do", "☀️", "33°/22°"), ("Fr", "🔥", "36°/25°") }),
                    ("Feuchte, neblige Morgen", new() { ("Mo", "🌫️", "20°/12°"), ("Di", "🌫️", "21°/13°"), ("Mi", "☀️", "23°/14°"), ("Do", "🌫️", "19°/11°"), ("Fr", "☁️", "20°/12°") })
                },
                ["September"] = new()
                {
                    ("Kühle, klare Herbsttage", new() { ("Mo", "☀️", "15°/5°"), ("Di", "☀️", "16°/6°"), ("Mi", "⛅", "14°/4°"), ("Do", "☀️", "17°/7°"), ("Fr", "☀️", "15°/5°") }),
                    ("Anhaltender Nieselregen", new() { ("Mo", "🌧️", "12°/8°"), ("Di", "🌧️", "11°/7°"), ("Mi", "🌧️", "10°/6°"), ("Do", "☁️", "11°/7°"), ("Fr", "🌧️", "12°/8°") }),
                    ("Erster Nachtfrost", new() { ("Mo", "❄️", "8°/-1°"), ("Di", "☀️", "9°/0°"), ("Mi", "❄️", "7°/-2°"), ("Do", "☀️", "10°/1°"), ("Fr", "❄️", "6°/-3°") })
                },
                ["Oktober"] = new()
                {
                    ("Bunter Blätterfall, milde Sonne", new() { ("Mo", "☀️", "14°/6°"), ("Di", "⛅", "13°/5°"), ("Mi", "💨", "12°/4°"), ("Do", "☀️", "15°/7°"), ("Fr", "☁️", "11°/3°") }),
                    ("Stürmisch und regnerisch", new() { ("Mo", "💨", "11°/7°"), ("Di", "🌧️", "10°/6°"), ("Mi", "⛈️", "12°/8°"), ("Do", "💨", "9°/5°"), ("Fr", "🌧️", "10°/6°") }),
                    ("Dichter Morgennebel", new() { ("Mo", "🌫️", "9°/3°"), ("Di", "🌫️", "8°/2°"), ("Mi", "⛅", "11°/4°"), ("Do", "🌫️", "7°/1°"), ("Fr", "☁️", "8°/2°") })
                },
                ["November"] = new()
                {
                    ("Kalt und grau, erster Schnee", new() { ("Mo", "☁️", "3°/-1°"), ("Di", "🌨️", "1°/-2°"), ("Mi", "🌨️", "0°/-3°"), ("Do", "🌧️", "2°/0°"), ("Fr", "☁️", "1°/-1°") }),
                    ("Dauerregen und Schlamm", new() { ("Mo", "🌧️", "5°/2°"), ("Di", "🌧️", "4°/1°"), ("Mi", "🌧️", "6°/3°"), ("Do", "☁️", "5°/2°"), ("Fr", "🌧️", "4°/1°") }),
                    ("Trockene, kalte Winde", new() { ("Mo", "💨", "2°/-4°"), ("Di", "💨", "1°/-5°"), ("Mi", "☀️", "3°/-3°"), ("Do", "💨", "0°/-6°"), ("Fr", "☁️", "1°/-4°") })
                },
                ["Dezember"] = new()
                {
                    ("Tiefer Schnee, Stille", new() { ("Mo", "❄️", "-2°/-8°"), ("Di", "🌨️", "-3°/-9°"), ("Mi", "❄️", "-4°/-10°"), ("Do", "🌨️", "-3°/-8°"), ("Fr", "❄️", "-5°/-11°") }),
                    ("Eisregen und gefrierende Nässe", new() { ("Mo", "🌧️", "0°/-3°"), ("Di", "❄️", "-1°/-4°"), ("Mi", "🌧️", "0°/-2°"), ("Do", "🌧️", "1°/-1°"), ("Fr", "🌧️", "-1°/-5°") }),
                    ("Klirrende Kälte bei Sternenhimmel", new() { ("Mo", "🌙", "-8°/-15°"), ("Di", "🌙", "-9°/-16°"), ("Mi", "🌙", "-10°/-17°"), ("Do", "💨", "-7°/-14°"), ("Fr", "🌙", "-9°/-16°") })
                }
            };

            foreach (var month in weatherData)
            {
                foreach (var (weatherName, forecast) in month.Value)
                {
                    var weatherOption = new WeatherOption
                    {
                        Month = month.Key,
                        WeatherName = weatherName,
                        CreatedAt = DateTime.Now
                    };

                    context.WeatherOptions.Add(weatherOption);
                    await context.SaveChangesAsync();

                    for (int i = 0; i < forecast.Count; i++)
                    {
                        var (day, icon, temp) = forecast[i];
                        var forecastDay = new WeatherForecastDay
                        {
                            WeatherOptionId = weatherOption.Id,
                            Day = day,
                            Icon = icon,
                            Temperature = temp,
                            DayOrder = i
                        };
                        context.WeatherForecastDays.Add(forecastDay);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
