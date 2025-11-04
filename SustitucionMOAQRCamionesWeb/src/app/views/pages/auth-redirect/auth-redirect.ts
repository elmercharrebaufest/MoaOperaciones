import { Component, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth-redirect',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './auth-redirect.html',
  styleUrls: ['./auth-redirect.scss']
})
export class AuthRedirectComponent implements OnInit, OnDestroy {
  countdown = signal(5);
  private intervalId: any;

  constructor(private router: Router) {}

  ngOnInit() {
    this.intervalId = setInterval(() => {
      const current = this.countdown();
      if (current > 0) {
        this.countdown.set(current - 1);
      } else {
        this.redirectToSearch();
      }
    }, 1000);
  }

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  redirectToSearch() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
    this.router.navigate(['/search']);
  }
}