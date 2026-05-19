// src/app/models/certificate.model.ts

export interface CertificateData {
  certificateId: string;
  studentName: string;
  courseCategory: string;
  courseType: string;
  levelDescription: string;
  courseName: string;
  startDate: string;
  endDate: string;
  duration: string;
  instructor: string;
  certificateType: string;
  qrCodeUrl: string;
  
  pdfUrl: string;
  imageUrl: string;
}

export interface VerifyResponse {
  isValid: boolean;
  message: string;
  data?: CertificateData;
  verifiedAt: string;
}