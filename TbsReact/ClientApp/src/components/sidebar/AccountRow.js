import { TableCell, TableRow } from "@mui/material";

const AccountRow = ({ accounts, handler, selected }) => {
    const rows = accounts.map(account => (
        <TableRow
            hover
            onClick={() => handler(account)}
            selected={account.id === selected}
            key = {account.id}>
            <TableCell>{account.name}</TableCell>
            <TableCell>{account.serverUrl}</TableCell>
        </TableRow>
    ))
    return (
        <>
            {rows}
        </>
    )
}

export default AccountRow;