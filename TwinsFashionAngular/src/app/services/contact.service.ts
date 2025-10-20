import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ContactRequest {
  emailAddress: string;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  constructor(private http: HttpClient) {}

  submitContact(request: ContactRequest) {
    return this.http.post(`${environment.apiBaseUrl}/api/Home/contacts`, request);
  }
}
