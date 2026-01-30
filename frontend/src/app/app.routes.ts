import { Routes } from '@angular/router';
import { Home } from './components/home/home';

export const routes: Routes = [
  { path: '', redirectTo: '/home', component: Home, pathMatch: 'full' },
  { path: 'home', component: Home },
];
