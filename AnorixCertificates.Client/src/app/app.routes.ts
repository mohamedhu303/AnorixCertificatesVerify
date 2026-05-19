import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'search',
    pathMatch: 'full',
  },
  {
    path: 'home',
    loadComponent: () =>
      import('./pages/search-page/search-page.component').then((m) => m.SearchPageComponent),
  },
  {
    path: 'search',
    loadComponent: () =>
      import('./pages/search-page/search-page.component').then((m) => m.SearchPageComponent),
  },
  {
    path: 'verify/:id',
    loadComponent: () => import('./pages/verify/verify.component').then((m) => m.VerifyComponent),
  },
  {
    path: '**',
    redirectTo: 'search',
  },
];
