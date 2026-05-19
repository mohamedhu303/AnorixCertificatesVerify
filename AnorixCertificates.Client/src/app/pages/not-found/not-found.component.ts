// src/app/pages/not-found/not-found.component.ts

import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss']
})
export class NotFoundComponent {}