import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CertificateService } from '../../services/certificate.service';
import { CertificateData } from '../../models/certificate.model';
import { HeaderComponent } from '../../components/header/header.component';
import { LoadingComponent } from '../../components/loading/loading.component';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-verify',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, LoadingComponent],
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements OnInit {

  certificate: CertificateData | null = null;
  isValid = false;
  isLoading = true;
  errorMessage = '';
  verifiedAt = '';

  // Image States
  imageUrl: SafeUrl | null = null;
  imageLoaded = false;
  imageError = false;
  imageLoading = true;

  constructor(
    private route: ActivatedRoute,
    private certService: CertificateService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.handleError('No certificate ID provided');
      return;
    }
    this.verifyCertificate(id);
  }

  private verifyCertificate(id: string): void {
    this.certService.verify(id).subscribe({
      next: (response) => {
        if (response.isValid && response.data) {
          this.certificate = response.data;
          this.isValid = true;
          this.verifiedAt = this.formatDateTime(response.verifiedAt);
          this.buildImageUrl();
        } else {
          this.handleError(response.message);
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Verification error:', err);
        this.handleError('Could not verify certificate');
        this.isLoading = false;
      }
    });
  }

  private buildImageUrl(): void {
    if (!this.certificate) {
      this.imageLoading = false;
      this.imageError = true;
      return;
    }

    const rawImageUrl =
      (this.certificate as any).imageUrl ||
      (this.certificate as any).certificateImageUrl ||
      (this.certificate as any).certificateImage ||
      (this.certificate as any).image ||
      '';

    if (!rawImageUrl) {
      this.imageLoading = false;
      this.imageLoaded = false;
      this.imageError = true;
      return;
    }

    const directImageUrl = this.convertGoogleDriveImageUrl(rawImageUrl);

    this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(directImageUrl);
    this.imageLoading = true;
    this.imageLoaded = false;
    this.imageError = false;
  }

  private convertGoogleDriveImageUrl(url: string): string {
    const trimmedUrl = url.trim();

    const patterns = [
      /\/file\/d\/([^/?]+)/,
      /[?&]id=([^&]+)/,
      /\/d\/([^/?]+)/,
      /\/uc\?id=([^&]+)/,
      /\/uc\?.*id=([^&]+)/
    ];

    for (const pattern of patterns) {
      const match = trimmedUrl.match(pattern);
      if (match && match[1]) {
        const fileId = match[1];
        return `https://drive.google.com/uc?export=view&id=${fileId}`;
      }
    }

    return trimmedUrl;
  }

  onImageLoad(): void {
    this.imageLoaded = true;
    this.imageLoading = false;
    this.imageError = false;
  }

  onImageError(): void {
    this.imageError = true;
    this.imageLoading = false;
    this.imageLoaded = false;
  }

  retryImageLoad(): void {
    if (!this.certificate) return;

    const rawImageUrl =
      (this.certificate as any).imageUrl ||
      (this.certificate as any).certificateImageUrl ||
      (this.certificate as any).certificateImage ||
      (this.certificate as any).image ||
      '';

    if (!rawImageUrl) {
      this.imageError = true;
      this.imageLoading = false;
      this.imageLoaded = false;
      return;
    }

    this.imageError = false;
    this.imageLoaded = false;
    this.imageLoading = true;

    const directImageUrl = this.convertGoogleDriveImageUrl(rawImageUrl);
    const separator = directImageUrl.includes('?') ? '&' : '?';
    const refreshedUrl = `${directImageUrl}${separator}t=${Date.now()}`;

    this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(refreshedUrl);
  }

  downloadPdf(): void {
    if (this.certificate?.pdfUrl) {
      window.open(this.certificate.pdfUrl, '_blank');
    }
  }

  private handleError(message: string): void {
    this.isValid = false;
    this.errorMessage = message;
    this.isLoading = false;
  }

  private formatDateTime(dateStr: string): string {
    try {
      const date = new Date(dateStr);
      return `${date.toLocaleDateString('en-US', {
        month: 'long',
        day: 'numeric',
        year: 'numeric'
      })} - ${date.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      })}`;
    } catch {
      return dateStr;
    }
  }
}
