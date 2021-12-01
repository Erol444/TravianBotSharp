import { TableCell, TableRow } from "@mui/material";

const AccountRow = ({acc, handler, selected}) => {
    return (
        <>
            <TableRow
                hover
                onClick={() => handler(acc)}
                selected = {acc.Id === selected}>
                <TableCell>{acc.Username}</TableCell>
                <TableCell>{acc.ServerUrl}</TableCell>
            </TableRow>
        </>
    )
}

export default AccountRow;