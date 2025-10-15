import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL } from '../../../shared/material';


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ...MATERIAL],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin-dashboard.scss']
})
export class AdminDashboardComponent {}
