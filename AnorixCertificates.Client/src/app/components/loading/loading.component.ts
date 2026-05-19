// src/app/components/loading/loading.component.ts

import { Component } from '@angular/core';

@Component({
  selector: 'app-loading',
  standalone: true,
  template: `
    <div class="loading-container">
      <div class="spinner"></div>
      <p class="loading-text">Verifying Certificate...</p>
    </div>
  `,
  styles: [`
    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 60vh;
      padding: 40px;
    }

    .spinner {
      width: 60px;
      height: 60px;
      border: 5px solid #f3f3f3;
      border-top: 5px solid #2563EB;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    .loading-text {
      margin-top: 20px;
      color: #64748B;
      font-size: 18px;
      font-family: 'Inter', sans-serif;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class LoadingComponent {}