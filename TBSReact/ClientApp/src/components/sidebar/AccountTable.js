import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import React, { useEffect, useState } from 'react'
import { getAccounts } from '../../api/Accounts/Account';
import AccountRow from './AccountRow'

const AccountTable = ({ selected, setSelected }) => {

    const [accounts, setAccounts] = useState([]);
    const onClick = (acc) => {
        setSelected(acc.id)
    };

    useEffect(() => {
        const fetchAccount = async () => {
            const data = await getAccounts();
            setAccounts(data)
        }
        fetchAccount();
    }, [selected]);

    return (
        <>
            <TableContainer>
                <Table
                    size="small"
                >
                    <TableHead>
                        <TableRow>
                            <TableCell>Username</TableCell>
                            <TableCell>Server url</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        <AccountRow accounts={accounts} handler={onClick} selected={selected}/>
                    </TableBody>
                </Table>
            </TableContainer>
        </>
    )
}

export default AccountTable;