import { Routes } from '@angular/router';
import { AuthComponent } from './pages/auth/auth.component';
import { ChatMainComponent } from './pages/chat-main/chat-main.component';
import { ContactsComponent } from './pages/contacts/contacts.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';

export const routes: Routes = [
    {
        path: '',
        component: AuthLayoutComponent,
        children: [
            { path: 'auth', component: AuthComponent },
            { path: '', redirectTo: '/auth', pathMatch: 'full' }
        ]
    },
    {
        path: '',
        component: MainLayoutComponent,
        children: [
            { path: 'inbox', component: ChatMainComponent },
            { path: 'contacts', component: ContactsComponent }
        ]
    },
    { path: '**', redirectTo: '/auth' }
];