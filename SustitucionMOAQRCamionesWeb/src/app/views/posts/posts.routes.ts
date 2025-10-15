import { Routes } from '@angular/router';

export const postsRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./posts-list/posts-list').then(m => m.PostsListComponent),
    title: 'Posts List'
  },
//   {
//     path: 'create',
//     loadComponent: () => import('./post-form/post-form').then(m => m.PostFormComponent),
//     title: 'Create Post'
//   },
//   {
//     path: ':id',
//     loadComponent: () => import('./post-detail/post-detail').then(m => m.PostDetailComponent),
//     title: 'Post Detail'
//   },
//   {
//     path: ':id/edit',
//     loadComponent: () => import('./post-form/post-form').then(m => m.PostFormComponent),
//     title: 'Edit Post'
//   }
];
