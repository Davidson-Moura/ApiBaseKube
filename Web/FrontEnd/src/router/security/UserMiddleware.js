import { useStore } from 'vuex';

export default function UserMiddleware(to, from, next) {
    const store = useStore();
    const isLogged = store.getters['sessionModule/IsLogged'];
    if (!isLogged) {
        next('/login');
    } else {
        next();
    }
}