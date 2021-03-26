import axios from 'axios';

export async function getListAccounts () {
    const { data } = await axios.get('/accounts');

    return data;
}
