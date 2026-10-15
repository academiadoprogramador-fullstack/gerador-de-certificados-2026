import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LucideAward, LucideFileText } from '@lucide/angular';

@Component({
  imports: [LucideFileText, LucideAward, RouterLink, RouterOutlet],
  selector: 'app-auth-shell',
  templateUrl: './auth-shell.html',
})
export class AuthShell {}
