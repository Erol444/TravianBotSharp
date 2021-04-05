import { getStatus } from '@/controller/DriverController';

export async function isLogin (index) {
    const status = await getStatus(index);

    if (status) return true;

    return false;
}

export function sleep (s) {
    return new Promise(resolve => setTimeout(resolve, s * 1000));
}
