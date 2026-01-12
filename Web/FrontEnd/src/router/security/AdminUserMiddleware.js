import { useStore } from 'vuex';

export default function UserMiddleware(to, from, next) {
    const store = useStore();
    const isAdmin = store.getters['sessionModule/IsAdmin'];
    if (!isAdmin) {
        next('/adminlogin');
    } else {
        next();
    }
}