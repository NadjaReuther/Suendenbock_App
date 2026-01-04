import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';

type ViewState = 'initialModal' | 'newActForm' | 'loadActView' | 'gameView' | 'manageActsView' | 'reselectWeatherView';

interface Akt {
  id: number;
  actNumber: number;
  mapImageUrl: string;
  country: string;
  companion1: string;
  companion2: string | null;
  month: string;
  weather: string;
}

const weatherData: Record<string, string[]> = {
  'Januar': ['Kalter, klarer Himmel', 'Anhaltender Schneefall', 'Beißender Wind und Frost'],
  'Februar': ['Grauer Himmel, Schneeregen', 'Überraschender Wärmeeinbruch', 'Eisige Nächte, sonnige Tage'],
  'März': ['Stürmische Winde, erster Regen', 'Langsame Schneeschmelze', 'Wechselhaft mit Sonne und Schauern'],
  'April': ['Heftige Regenschauer', 'Milder Frühlingsbeginn', 'Später Frosteinbruch'],
  'Mai': ['Warme, sonnige Tage', 'Gewitter am Nachmittag', 'Kühle, feuchte Morgen'],
  'Juni': ['Heiß und trocken', 'Schwüle mit häufigen Gewittern', 'Angenehm warm mit leichter Brise'],
  'Juli': ['Drückende Hitze, Dürre', 'Kurze, heftige Sommergewitter', 'Warme Nächte unter klarem Himmel'],
  'August': ['Erntewetter, goldenes Licht', 'Hitzewelle mit Waldbrandgefahr', 'Feuchte, neblige Morgen'],
  'September': ['Kühle, klare Herbsttage', 'Anhaltender Nieselregen', 'Erster Nachtfrost'],
  'Oktober': ['Bunter Blätterfall, milde Sonne', 'Stürmisch und regnerisch', 'Dichter Morgennebel'],
  'November': ['Kalt und grau, erster Schnee', 'Dauerregen und Schlamm', 'Trockene, kalte Winde'],
  'Dezember': ['Tiefer Schnee, Stille', 'Eisregen und gefrierende Nässe', 'Klirrende Kälte bei Sternenhimmel'],
};

interface DailyForecast {
  day: string;
  icon: string;
  temp: string;
}

const weatherForecasts: Record<string, DailyForecast[]> = {
  'Kalter, klarer Himmel': [ { day: 'Mo', icon: 'clear_day', temp: '2°/-5°' }, { day: 'Di', icon: 'clear_day', temp: '1°/-6°' }, { day: 'Mi', icon: 'clear_day', temp: '2°/-5°' }, { day: 'Do', icon: 'wb_cloudy', temp: '0°/-4°' }, { day: 'Fr', icon: 'clear_day', temp: '3°/-3°' } ],
  'Anhaltender Schneefall': [ { day: 'Mo', icon: 'cloudy_snowing', temp: '-1°/-4°' }, { day: 'Di', icon: 'cloudy_snowing', temp: '-2°/-5°' }, { day: 'Mi', icon: 'ac_unit', temp: '-3°/-7°' }, { day: 'Do', icon: 'cloudy_snowing', temp: '-2°/-5°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '0°/-3°' } ],
  'Beißender Wind und Frost': [ { day: 'Mo', icon: 'air', temp: '-5°/-10°' }, { day: 'Di', icon: 'air', temp: '-6°/-12°' }, { day: 'Mi', icon: 'ac_unit', temp: '-7°/-14°' }, { day: 'Do', icon: 'air', temp: '-6°/-11°' }, { day: 'Fr', icon: 'clear_day', temp: '-4°/-8°' } ],
  'Grauer Himmel, Schneeregen': [ { day: 'Mo', icon: 'sleet', temp: '1°/-2°' }, { day: 'Di', icon: 'sleet', temp: '0°/-3°' }, { day: 'Mi', icon: 'cloudy_snowing', temp: '-1°/-4°' }, { day: 'Do', icon: 'rainy', temp: '2°/0°' }, { day: 'Fr', icon: 'sleet', temp: '1°/-1°' } ],
  'Überraschender Wärmeeinbruch': [ { day: 'Mo', icon: 'wb_sunny', temp: '10°/2°' }, { day: 'Di', icon: 'wb_sunny', temp: '12°/3°' }, { day: 'Mi', icon: 'rainy', temp: '8°/1°' }, { day: 'Do', icon: 'cloudy', temp: '7°/0°' }, { day: 'Fr', icon: 'wb_sunny', temp: '11°/2°' } ],
  'Eisige Nächte, sonnige Tage': [ { day: 'Mo', icon: 'clear_day', temp: '4°/-4°' }, { day: 'Di', icon: 'clear_day', temp: '5°/-5°' }, { day: 'Mi', icon: 'clear_day', temp: '6°/-4°' }, { day: 'Do', icon: 'clear_day', temp: '5°/-3°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '3°/-2°' } ],
  'Stürmische Winde, erster Regen': [ { day: 'Mo', icon: 'air', temp: '8°/3°' }, { day: 'Di', icon: 'rainy', temp: '7°/4°' }, { day: 'Mi', icon: 'rainy', temp: '6°/2°' }, { day: 'Do', icon: 'air', temp: '9°/3°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '7°/1°' } ],
  'Langsame Schneeschmelze': [ { day: 'Mo', icon: 'wb_cloudy', temp: '3°/0°' }, { day: 'Di', icon: 'rainy', temp: '4°/1°' }, { day: 'Mi', icon: 'foggy', temp: '2°/0°' }, { day: 'Do', icon: 'wb_sunny', temp: '5°/1°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '4°/0°' } ],
  'Wechselhaft mit Sonne und Schauern': [ { day: 'Mo', icon: 'partly_cloudy_day', temp: '10°/4°' }, { day: 'Di', icon: 'rainy', temp: '8°/3°' }, { day: 'Mi', icon: 'wb_sunny', temp: '12°/5°' }, { day: 'Do', icon: 'rainy', temp: '9°/4°' }, { day: 'Fr', icon: 'partly_cloudy_day', temp: '11°/5°' } ],
  'Heftige Regenschauer': [ { day: 'Mo', icon: 'rainy', temp: '9°/5°' }, { day: 'Di', icon: 'rainy', temp: '8°/4°' }, { day: 'Mi', icon: 'thunderstorm', temp: '10°/6°' }, { day: 'Do', icon: 'rainy', temp: '7°/3°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '8°/4°' } ],
  'Milder Frühlingsbeginn': [ { day: 'Mo', icon: 'wb_sunny', temp: '14°/6°' }, { day: 'Di', icon: 'partly_cloudy_day', temp: '15°/7°' }, { day: 'Mi', icon: 'wb_sunny', temp: '16°/8°' }, { day: 'Do', icon: 'wb_cloudy', temp: '13°/6°' }, { day: 'Fr', icon: 'rainy', temp: '12°/5°' } ],
  'Später Frosteinbruch': [ { day: 'Mo', icon: 'ac_unit', temp: '5°/-2°' }, { day: 'Di', icon: 'ac_unit', temp: '4°/-3°' }, { day: 'Mi', icon: 'clear_day', temp: '6°/-1°' }, { day: 'Do', icon: 'cloudy', temp: '5°/0°' }, { day: 'Fr', icon: 'sleet', temp: '3°/-1°' } ],
  'Warme, sonnige Tage': [ { day: 'Mo', icon: 'wb_sunny', temp: '22°/12°' }, { day: 'Di', icon: 'wb_sunny', temp: '24°/14°' }, { day: 'Mi', icon: 'clear_day', temp: '25°/15°' }, { day: 'Do', icon: 'wb_sunny', temp: '23°/13°' }, { day: 'Fr', icon: 'partly_cloudy_day', temp: '21°/12°' } ],
  'Gewitter am Nachmittag': [ { day: 'Mo', icon: 'partly_cloudy_day', temp: '23°/14°' }, { day: 'Di', icon: 'thunderstorm', temp: '20°/15°' }, { day: 'Mi', icon: 'partly_cloudy_day', temp: '24°/16°' }, { day: 'Do', icon: 'thunderstorm', temp: '21°/15°' }, { day: 'Fr', icon: 'wb_sunny', temp: '25°/17°' } ],
  'Kühle, feuchte Morgen': [ { day: 'Mo', icon: 'foggy', temp: '18°/10°' }, { day: 'Di', icon: 'foggy', temp: '19°/11°' }, { day: 'Mi', icon: 'wb_sunny', temp: '21°/12°' }, { day: 'Do', icon: 'foggy', temp: '17°/9°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '18°/10°' } ],
  'Heiß und trocken': [ { day: 'Mo', icon: 'clear_day', temp: '28°/18°' }, { day: 'Di', icon: 'clear_day', temp: '30°/20°' }, { day: 'Mi', icon: 'clear_day', temp: '31°/21°' }, { day: 'Do', icon: 'clear_day', temp: '29°/19°' }, { day: 'Fr', icon: 'clear_day', temp: '30°/20°' } ],
  'Schwüle mit häufigen Gewittern': [ { day: 'Mo', icon: 'thunderstorm', temp: '26°/19°' }, { day: 'Di', icon: 'cloudy', temp: '27°/20°' }, { day: 'Mi', icon: 'thunderstorm', temp: '25°/18°' }, { day: 'Do', icon: 'thunderstorm', temp: '26°/19°' }, { day: 'Fr', icon: 'rainy', temp: '24°/18°' } ],
  'Angenehm warm mit leichter Brise': [ { day: 'Mo', icon: 'partly_cloudy_day', temp: '24°/16°' }, { day: 'Di', icon: 'air', temp: '23°/15°' }, { day: 'Mi', icon: 'wb_sunny', temp: '25°/17°' }, { day: 'Do', icon: 'air', temp: '22°/14°' }, { day: 'Fr', icon: 'partly_cloudy_day', temp: '24°/16°' } ],
  'Drückende Hitze, Dürre': [ { day: 'Mo', icon: 'clear_day', temp: '33°/22°' }, { day: 'Di', icon: 'clear_day', temp: '35°/24°' }, { day: 'Mi', icon: 'clear_day', temp: '36°/25°' }, { day: 'Do', icon: 'clear_day', temp: '34°/23°' }, { day: 'Fr', icon: 'whatshot', temp: '37°/26°' } ],
  'Kurze, heftige Sommergewitter': [ { day: 'Mo', icon: 'wb_sunny', temp: '30°/20°' }, { day: 'Di', icon: 'thunderstorm', temp: '28°/19°' }, { day: 'Mi', icon: 'wb_sunny', temp: '31°/21°' }, { day: 'Do', icon: 'thunderstorm', temp: '29°/20°' }, { day: 'Fr', icon: 'wb_sunny', temp: '32°/22°' } ],
  'Warme Nächte unter klarem Himmel': [ { day: 'Mo', icon: 'clear_night', temp: '28°/19°' }, { day: 'Di', icon: 'clear_night', temp: '29°/20°' }, { day: 'Mi', icon: 'clear_night', temp: '30°/21°' }, { day: 'Do', icon: 'clear_night', temp: '28°/19°' }, { day: 'Fr', icon: 'clear_night', temp: '29°/20°' } ],
  'Erntewetter, goldenes Licht': [ { day: 'Mo', icon: 'wb_sunny', temp: '25°/15°' }, { day: 'Di', icon: 'wb_sunny', temp: '26°/16°' }, { day: 'Mi', icon: 'clear_day', temp: '27°/17°' }, { day: 'Do', icon: 'partly_cloudy_day', temp: '24°/14°' }, { day: 'Fr', icon: 'wb_sunny', temp: '25°/15°' } ],
  'Hitzewelle mit Waldbrandgefahr': [ { day: 'Mo', icon: 'whatshot', temp: '32°/21°' }, { day: 'Di', icon: 'whatshot', temp: '34°/23°' }, { day: 'Mi', icon: 'whatshot', temp: '35°/24°' }, { day: 'Do', icon: 'clear_day', temp: '33°/22°' }, { day: 'Fr', icon: 'whatshot', temp: '36°/25°' } ],
  'Feuchte, neblige Morgen': [ { day: 'Mo', icon: 'foggy', temp: '20°/12°' }, { day: 'Di', icon: 'foggy', temp: '21°/13°' }, { day: 'Mi', icon: 'wb_sunny', temp: '23°/14°' }, { day: 'Do', icon: 'foggy', temp: '19°/11°' }, { day: 'Fr', icon: 'wb_cloudy', temp: '20°/12°' } ],
  'Kühle, klare Herbsttage': [ { day: 'Mo', icon: 'clear_day', temp: '15°/5°' }, { day: 'Di', icon: 'clear_day', temp: '16°/6°' }, { day: 'Mi', icon: 'partly_cloudy_day', temp: '14°/4°' }, { day: 'Do', icon: 'clear_day', temp: '17°/7°' }, { day: 'Fr', icon: 'clear_day', temp: '15°/5°' } ],
  'Anhaltender Nieselregen': [ { day: 'Mo', icon: 'rainy', temp: '12°/8°' }, { day: 'Di', icon: 'rainy', temp: '11°/7°' }, { day: 'Mi', icon: 'rainy', temp: '10°/6°' }, { day: 'Do', icon: 'cloudy', temp: '11°/7°' }, { day: 'Fr', icon: 'rainy', temp: '12°/8°' } ],
  'Erster Nachtfrost': [ { day: 'Mo', icon: 'ac_unit', temp: '8°/-1°' }, { day: 'Di', icon: 'clear_day', temp: '9°/0°' }, { day: 'Mi', icon: 'ac_unit', temp: '7°/-2°' }, { day: 'Do', icon: 'clear_day', temp: '10°/1°' }, { day: 'Fr', icon: 'ac_unit', temp: '6°/-3°' } ],
  'Bunter Blätterfall, milde Sonne': [ { day: 'Mo', icon: 'wb_sunny', temp: '14°/6°' }, { day: 'Di', icon: 'partly_cloudy_day', temp: '13°/5°' }, { day: 'Mi', icon: 'air', temp: '12°/4°' }, { day: 'Do', icon: 'wb_sunny', temp: '15°/7°' }, { day: 'Fr', icon: 'cloudy', temp: '11°/3°' } ],
  'Stürmisch und regnerisch': [ { day: 'Mo', icon: 'air', temp: '11°/7°' }, { day: 'Di', icon: 'rainy', temp: '10°/6°' }, { day: 'Mi', icon: 'thunderstorm', temp: '12°/8°' }, { day: 'Do', icon: 'air', temp: '9°/5°' }, { day: 'Fr', icon: 'rainy', temp: '10°/6°' } ],
  'Dichter Morgennebel': [ { day: 'Mo', icon: 'foggy', temp: '9°/3°' }, { day: 'Di', icon: 'foggy', temp: '8°/2°' }, { day: 'Mi', icon: 'partly_cloudy_day', temp: '11°/4°' }, { day: 'Do', icon: 'foggy', temp: '7°/1°' }, { day: 'Fr', icon: 'cloudy', temp: '8°/2°' } ],
  'Kalt und grau, erster Schnee': [ { day: 'Mo', icon: 'cloudy', temp: '3°/-1°' }, { day: 'Di', icon: 'cloudy_snowing', temp: '1°/-2°' }, { day: 'Mi', icon: 'cloudy_snowing', temp: '0°/-3°' }, { day: 'Do', icon: 'sleet', temp: '2°/0°' }, { day: 'Fr', icon: 'cloudy', temp: '1°/-1°' } ],
  'Dauerregen und Schlamm': [ { day: 'Mo', icon: 'rainy', temp: '5°/2°' }, { day: 'Di', icon: 'rainy', temp: '4°/1°' }, { day: 'Mi', icon: 'rainy', temp: '6°/3°' }, { day: 'Do', icon: 'cloudy', temp: '5°/2°' }, { day: 'Fr', icon: 'rainy', temp: '4°/1°' } ],
  'Trockene, kalte Winde': [ { day: 'Mo', icon: 'air', temp: '2°/-4°' }, { day: 'Di', icon: 'air', temp: '1°/-5°' }, { day: 'Mi', icon: 'clear_day', temp: '3°/-3°' }, { day: 'Do', icon: 'air', temp: '0°/-6°' }, { day: 'Fr', icon: 'cloudy', temp: '1°/-4°' } ],
  'Tiefer Schnee, Stille': [ { day: 'Mo', icon: 'ac_unit', temp: '-2°/-8°' }, { day: 'Di', icon: 'cloudy_snowing', temp: '-3°/-9°' }, { day: 'Mi', icon: 'ac_unit', temp: '-4°/-10°' }, { day: 'Do', icon: 'cloudy_snowing', temp: '-3°/-8°' }, { day: 'Fr', icon: 'ac_unit', temp: '-5°/-11°' } ],
  // FIX: Corrected the malformed object for Wednesday's forecast. The 'icon' property was missing.
  'Eisregen und gefrierende Nässe': [ { day: 'Mo', icon: 'sleet', temp: '0°/-3°' }, { day: 'Di', icon: 'ac_unit', temp: '-1°/-4°' }, { day: 'Mi', icon: 'sleet', temp: '0°/-2°' }, { day: 'Do', icon: 'rainy', temp: '1°/-1°' }, { day: 'Fr', icon: 'sleet', temp: '-1°/-5°' } ],
  'Klirrende Kälte bei Sternenhimmel': [ { day: 'Mo', icon: 'clear_night', temp: '-8°/-15°' }, { day: 'Di', icon: 'clear_night', temp: '-9°/-16°' }, { day: 'Mi', icon: 'clear_night', temp: '-10°/-17°' }, { day: 'Do', icon: 'air', temp: '-7°/-14°' }, { day: 'Fr', icon: 'clear_night', temp: '-9°/-16°' } ],
};


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  viewState = signal<ViewState>('initialModal');
  savedAkten = signal<Akt[]>([]);
  activeAkt = signal<Akt | null>(null);
  actToProcess = signal<Akt | null>(null);
  weatherForecasts = weatherForecasts;
  
  // Form state signals
  actNumber = signal<number | null>(null);
  mapPreviewUrl = signal<string | null>(null);
  companionOptions = ['Okko Brand', 'Emma Landter', 'Felix Schnee'];
  selectedCompanion1 = signal<string | null>(null);
  selectedCompanion2 = signal<string | null>(null);
  selectedCountry = signal<string>('Böhmen');
  selectedMonth = signal<string | null>(null);
  selectedWeather = signal<string | null>(null);

  isCompanionSelectionValid = computed(() => !!this.selectedCompanion1());
  
  isFormFullyValid = computed(() => {
    return this.actNumber() !== null &&
           this.mapPreviewUrl() !== null &&
           this.isCompanionSelectionValid() &&
           this.selectedCountry() !== null &&
           this.selectedMonth() !== null &&
           this.selectedWeather() !== null;
  });

  companion2Options = computed(() => {
    const firstSelection = this.selectedCompanion1();
    if (!firstSelection) {
      return [];
    }
    return this.companionOptions.filter(c => c !== firstSelection);
  });

  weatherOptions = computed(() => {
    const month = this.selectedMonth();
    return month ? weatherData[month] ?? [] : [];
  });
  
  activeWeatherForecast = computed(() => {
    const akt = this.activeAkt();
    if (!akt) return null;
    return this.weatherForecasts[akt.weather] ?? null;
  });

  selectedWeatherForecast = computed(() => {
      const weather = this.selectedWeather();
      if (!weather) return null;
      return this.weatherForecasts[weather] ?? null;
  });

  neuenAktAnlegen(): void {
    this.resetForm();
    this.actToProcess.set(null);
    this.viewState.set('newActForm');
  }

  vorhandenenAktLaden(): void {
    this.viewState.set('loadActView');
  }

  verwaltenAkten(): void {
    this.viewState.set('manageActsView');
  }
  
  private resetForm(): void {
    this.actNumber.set(null);
    this.mapPreviewUrl.set(null);
    this.selectedCompanion1.set(null);
    this.selectedCompanion2.set(null);
    this.selectedCountry.set('Böhmen');
    this.selectedMonth.set(null);
    this.selectedWeather.set(null);
  }

  saveAkt(): void {
    if (!this.isFormFullyValid()) {
      console.error('Form is invalid.');
      return;
    }
    const aktData = {
      actNumber: this.actNumber()!,
      mapImageUrl: this.mapPreviewUrl()!,
      country: this.selectedCountry(),
      companion1: this.selectedCompanion1()!,
      companion2: this.selectedCompanion2(),
      month: this.selectedMonth()!,
      weather: this.selectedWeather()!
    };

    const existingAkt = this.actToProcess();
    if (existingAkt) {
      this.savedAkten.update(akten => 
        akten.map(a => a.id === existingAkt.id ? { ...a, ...aktData } : a)
      );
    } else {
      const newAkt: Akt = { id: Date.now(), ...aktData };
      this.savedAkten.update(akten => [...akten, newAkt]);
    }
    
    this.resetForm();
    this.actToProcess.set(null);
    this.viewState.set('loadActView');
  }

  startEditAkt(akt: Akt): void {
    this.actToProcess.set(akt);
    this.actNumber.set(akt.actNumber);
    this.mapPreviewUrl.set(akt.mapImageUrl);
    this.selectedCountry.set(akt.country);
    this.selectedCompanion1.set(akt.companion1);
    this.selectedCompanion2.set(akt.companion2);
    this.selectedMonth.set(akt.month);
    this.selectedWeather.set(akt.weather);
    this.viewState.set('newActForm');
  }

  deleteAkt(aktId: number): void {
    if (confirm('Diesen Akt wirklich endgültig löschen?')) {
      this.savedAkten.update(akten => akten.filter(a => a.id !== aktId));
    }
  }

  startReselectWeather(akt: Akt): void {
    this.actToProcess.set(akt);
    this.selectedMonth.set(akt.month);
    this.selectedWeather.set(null);
    this.viewState.set('reselectWeatherView');
  }

  startGame(): void {
    const aktToStart = this.actToProcess();
    const newWeather = this.selectedWeather();
    if (!aktToStart || !newWeather) return;

    const sessionAkt: Akt = { ...aktToStart, weather: newWeather };
    this.activeAkt.set(sessionAkt);
    this.viewState.set('gameView');
    
    this.actToProcess.set(null);
    this.resetForm();
  }

  backToInitialModal(): void {
    this.viewState.set('initialModal');
    this.actToProcess.set(null);
    this.resetForm();
  }
  
  backToLoadViewFromWeather(): void {
    this.viewState.set('loadActView');
    this.actToProcess.set(null);
    this.selectedMonth.set(null);
    this.selectedWeather.set(null);
  }

  goBackToLoadView(): void {
    this.activeAkt.set(null);
    this.viewState.set('loadActView');
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => this.mapPreviewUrl.set(e.target?.result as string);
      reader.readAsDataURL(file);
    }
  }

  onActNumberChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.actNumber.set(value ? parseInt(value, 10) : null);
  }

  onCountryChange(event: Event): void {
     this.selectedCountry.set((event.target as HTMLSelectElement).value);
  }

  onCompanion1Change(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value === 'null' ? null : select.value;
    this.selectedCompanion1.set(value);
    
    if (!value) {
      this.selectedCompanion2.set(null);
    } else if (value === this.selectedCompanion2()) {
      this.selectedCompanion2.set(null);
    }
  }

  onCompanion2Change(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value === 'null' ? null : select.value;
    this.selectedCompanion2.set(value);
  }

  onMonthChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.selectedMonth.set(select.value);
    this.selectedWeather.set(null);
  }
  
  onWeatherChange(event: Event): void {
    this.selectedWeather.set((event.target as HTMLInputElement).value);
  }
}
