import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HeaderComponent],
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.scss']
})
export class SearchPageComponent {
  certificateId = '';
  hasError = false;
  errorMessage = '';

  constructor(private router: Router) {}

  searchCertificate(): void {
    const id = this.certificateId.trim();

    if (!id) {
      this.hasError = true;
      this.errorMessage = 'Please enter a certificate ID';
      return;
    }

    this.hasError = false;
    this.errorMessage = '';
    this.router.navigate(['/verify', id]);
  }

  clearError(): void {
    if (this.hasError) {
      this.hasError = false;
      this.errorMessage = '';
    }
  }
}