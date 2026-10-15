import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { CursosShell } from './cursos/shell/cursos-shell';

@Component({
  imports: [RouterOutlet, CursosShell],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {}
