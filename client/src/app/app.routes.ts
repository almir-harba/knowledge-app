import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/skill-list/skill-list').then((m) => m.SkillList),
  },
  {
    path: 'skills/new',
    loadComponent: () => import('./features/skill-form/skill-form').then((m) => m.SkillForm),
  },
  {
    path: 'skills/:id/edit',
    loadComponent: () => import('./features/skill-form/skill-form').then((m) => m.SkillForm),
  },
  {
    path: 'skills/:id',
    loadComponent: () =>
      import('./features/skill-detail/skill-detail').then((m) => m.SkillDetail),
  },
  { path: '**', redirectTo: '' },
];
