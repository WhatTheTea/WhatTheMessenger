import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Chats } from './features/chats/chats';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'chats',
    component: Chats,
  },
  {
    path: 'chats/:id',
    component: Chats,
  },
];
