import { TableCell, TableRow } from "@mui/material";

const AccountRow = ({access, handler, selected}) => {
    return (
        <>
            <TableRow
                hover
                onClick={() => handler(access)}
                selected = {access.id === selected}>
                <TableCell>{[access.proxy.ip, ':', access.proxy.port].join()}</TableCell>
                <TableCell>{access.proxy.username}</TableCell>
                <TableCell>{access.proxy.OK === true ? "✔" : "❌"}</TableCell>
            </TableRow>
        </>
    )
}

export default AccountRow;