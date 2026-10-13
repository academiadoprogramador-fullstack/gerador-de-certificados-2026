import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight, LucideAward, LucideSparkles } from '@lucide/angular';

@Component({
  imports: [LucideAward, LucideSparkles, LucideArrowRight, RouterLink],
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {}
