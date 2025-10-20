import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ContactService, ContactRequest } from '../services/contact.service';

@Component({
  selector: 'app-contact',
  standalone: false,
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  formData = {
    emailAddress: '',
    description: ''
  };
  submitting = false;
  submitSuccess = false;
  submitError = false;

  constructor(private contactService: ContactService) {}

  submitContact(form: NgForm): void {
    if (this.submitting || form.invalid) {
      this.submitError = true;
      return;
    }

    this.submitting = true;
    this.submitError = false;
    this.submitSuccess = false;

    this.contactService.submitContact(this.formData).subscribe({
      next: () => {
        this.submitting = false;
        this.submitSuccess = true;
        this.formData = { emailAddress: '', description: '' };
        form.resetForm();
      },
      error: () => {
        this.submitting = false;
        this.submitError = true;
      }
    });
  }
}
