import { createRouter, createWebHistory } from 'vue-router'
import SuperUserMiddleware from '@/router/security/SuperUserMiddleware.js'
import UserMiddleware from '@/router/security/UserMiddleware.js'
import AdminUserMiddleware from '@/router/security/AdminUserMiddleware.js'

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'Login',
        alias: 'index.html',
        component: () => import('@/views/Login.vue'),
      },
      {
        path: 'adminlogin',
        name: 'AdminLogin',
        component: () => import('@/views/security/AdminLoginView.vue'),
      },
      {
        path: 'app',
        name: 'App',
        meta: {
          middleware: [UserMiddleware]
        },
        component: () => import('@/views/AppDefault.vue'),
        children: [
          {
            path: '',
            name: 'Home',
            component: () => import('@/views/Home.vue'),
          },
        ]
      },
      {
        path: 'admin',
        name: 'Admin',
        meta: {
          middleware: [AdminUserMiddleware]
        },
        component: () => import('@/views/admin/AppAdminDefault.vue'),
        children: [
          {
            path: '',
            name: 'AdminHome',
            component: () => import('@/views/admin/AdminHome.vue'),
          },
        ]
      },
      
      
    ],
  },
  { path: '/:catchAll(.*)', redirect: '/' }
]


const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
})
router.beforeEach((to, from, next) => {
  const middleware = to.meta.middleware;
  if (middleware) {
    middleware.forEach(m => m(to, from, next));
  } else {
    next();
  }
});

export default router
