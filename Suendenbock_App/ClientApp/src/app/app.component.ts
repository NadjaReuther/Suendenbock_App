import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'ClientApp';

  constructor(private router: Router) {}

  ngOnInit() {
    // Beim Start zur entsprechenden Route navigieren
    // Damit Angular nicht versucht, die ASP.NET Route zu matchen
    const currentPath = window.location.pathname;
    const currentHash = window.location.hash;

    // Wenn wir auf der ASP.NET Route sind, navigiere zu Angular-Route
    if (currentPath.includes('Player/Weihnachtenuebersicht')) {
      // Prüfe ob ein Hash gesetzt ist (z.B. #/raetsel)
      if (currentHash && currentHash.startsWith('#/')) {
        const route = currentHash.substring(2); // Entferne '#/'
        this.router.navigate([route]);
      } else {
        // Kein Hash -> Standard zur Weihnachten-Route
        this.router.navigate(['/weihnachten']);
      }
    }
  }
}
