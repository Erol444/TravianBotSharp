import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import React from 'react'
import AccountRow from './AccountRow'

const AccountTable = ({ selected, setSelected }) => {
    const onClick = (acc) => {
        setSelected(acc.Id)
    };
    const accs = [
        { Id: 0, Username: "alsdao", ServerUrl: "ts50.x5.america.travian.com" },
        { Id: 1, Username: "alo", ServerUrl: "sadasd" },
        { Id: 2, Username: "ao", ServerUrl: "sadasd" },
        { Id: 3, Username: "a2lo", ServerUrl: "sadasd" },
        { Id: 4, Username: "al3o", ServerUrl: "sadasd" },
        { Id: 5, Username: "al4o", ServerUrl: "sadasd" },
        { Id: 6, Username: "al5o", ServerUrl: "sadasd" },
    ]

    const accsNode = accs.map((acc) => (
        <AccountRow acc={acc} handler={onClick} key={acc.Id} selected={selected} />)
    )

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
                            {accsNode}
                        </TableBody>
                    </Table>
                </TableContainer>
        </>
    )
}

export default AccountTable;