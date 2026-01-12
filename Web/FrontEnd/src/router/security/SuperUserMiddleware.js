import { useStore } from 'vuex';

export default function SuperUserMiddleware(to, from, next) {
    const store = useStore();
    const isSuper = store.getters['sessionModule/IsSuper'];
    if (!isSuper) {
        next('/login');
    } else {
        next();
    }
}