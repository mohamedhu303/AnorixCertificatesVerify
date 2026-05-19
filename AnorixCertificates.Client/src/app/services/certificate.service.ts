// src/app/services/certificate.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { VerifyResponse } from '../models/certificate.model';

@Injectable({
  providedIn: 'root'
})
export class CertificateService {
  private apiUrl = 'https://localhost:7149/api/certificates';

  constructor(private http: HttpClient) {}

  verify(id: string): Observable<VerifyResponse> {
    return this.http.get<VerifyResponse>(
      `${this.apiUrl}/verify/${id}`
    ).pipe(
      catchError(error => {
        console.error('API Error:', error);
        return of({
          isValid: false,
          message: 'Could not connect to server',
          verifiedAt: new Date().toISOString()
        } as VerifyResponse);
      })
    );
  }
}