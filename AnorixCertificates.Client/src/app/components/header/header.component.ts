import { CommonModule, Location } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  constructor(
    private location: Location,
    private router: Router,
  ) {}

  get showBackButton(): boolean {
    return !this.router.url.startsWith('/search');
  }

  goBack(): void {
    try {
      const referrer = document.referrer;
      if (referrer) {
        const referrerUrl = new URL(referrer);
        if (referrerUrl.origin === window.location.origin) {
          this.location.back();
          return;
        }
      }
    } catch {}

    this.router.navigate(['/search']);
  }
}
