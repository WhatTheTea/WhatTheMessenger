import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { ChatList } from './features/chats/chat-list';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'chats',
    component: ChatList,
  },
  {
    path: 'chats/:id',
    component: ChatList,
  },
];
