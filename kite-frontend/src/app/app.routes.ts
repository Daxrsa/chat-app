import { Routes } from '@angular/router';
import { AuthComponent } from './pages/auth/auth.component';
import { ChatMainComponent } from './pages/chat-main/chat-main.component';
import { ContactsComponent } from './pages/contacts/contacts.component';

export const routes: Routes = [
    { path: 'auth', component: AuthComponent },
    { path: 'inbox', component: ChatMainComponent },
    { path: 'contacts', component: ContactsComponent },
    { path: '', redirectTo: '/auth', pathMatch: 'full' }, 
    { path: '**', redirectTo: '/auth' } 
];