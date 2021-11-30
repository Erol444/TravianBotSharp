import { useEffect, useState } from 'react';
import { Modal, Button, Box, Typography, Input, Table, TableHead, TableCell } from '@mui/material';
import AccessRow from './AccessRow';

const style = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    bgcolor: 'background.paper',
    border: '2px solid #000',
    boxShadow: 24,
    p: 4,
};

const AccountModal = ({ editMode = false, accID = -1 }) => {
    const [selected, setSelected] = useState(0);
    const [open, setOpen] = useState(false);
    const [accesses, setAccesses] = useState([]);

    //Form
    const [username, setUsername] = useState("");
    const [server, setServer] = useState("");
    const [password, setPassword] = useState("");
    const [proxyIP, setProxyIP] = useState(0);
    const [proxyPort, setProxyPort] = useState("");
    const [proxyUsername, setProxyUsername] = useState("");
    const [proxyPassword, setProxyPassword] = useState("");


    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
    const onClickTable = (access) => {
        setSelected(access.id)
    };
    const onClickAdd = () => {
        const newAccess = { id: accesses.length, password: password, proxy: { ip: proxyIP, port: proxyPort, username: proxyUsername, password: proxyPassword, ok: false } }
        setAccesses([...accesses, newAccess])
        console.log(newAccess)
        update();
    }
    const onClickEdit = () => {
        const newAccess = { id: selected, password: password, proxy: { ip: proxyIP, port: proxyPort, username: proxyUsername, password: proxyPassword, ok: false } }
        setAccesses( (old) => {old[selected] = newAccess; return old})
        console.log(newAccess)
        update();

    }
    const onClickDelete = () => {
        console.log("delete", selected);
        update();

    }
    useEffect(() => {
        console.log("access", selected)
    }, [selected]);

    let accessNode = null;

    const update = () => {
        accessNode = accesses.map((access) => (
            <AccessRow access={access} handler={onClickTable} key={access.Id} selected={selected} />)
        );
    }
   
    

    return (
        <>
            <Button onClick={handleOpen}>{editMode === true ? "Add account" : "Edit"} </Button>
            <Modal
                open={open}
                onClose={handleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description">
                <Box sx={style}>
                    <Typography id="modal-modal-title" variant="h6" component="h2">
                        {editMode === true ? "Add account" : "Edit account"}
                    </Typography>
                    <table>
                        <tr>
                            <td>
                                Username
                            </td>
                            <td>
                                <Input autoFocus={true} value={username} onInput={e => setUsername(e.target.value)} />
                            </td>
                            <td>
                                Server
                            </td>
                            <td>
                                <Input value={server} onInput={e => setServer(e.target.value)} />
                            </td>
                        </tr>
                        <tr>
                        </tr>
                        <td>
                            Password
                        </td>
                        <td>
                            <Input type="password" value={password} onInput={e => setPassword(e.target.value)} />
                        </td>
                        <tr>
                            <td>
                                Proxy IP
                            </td>
                            <td>
                                <Input value={proxyIP} onInput={e => setProxyIP(e.target.value)} />
                            </td>
                            <td>
                                Proxy username
                            </td>
                            <td>
                                <Input value={proxyUsername} onInput={e => setProxyUsername(e.target.value)} />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Proxy Port
                            </td>
                            <td>
                                <Input type="number" value={proxyPort} onInput={e => setProxyPort(e.target.value)} />
                            </td>
                            <td>
                                Proxy password
                            </td>
                            <td>
                                <Input value={proxyPassword} onInput={e => setProxyPassword(e.target.value)} />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <td>
                            <Button onClick={onClickAdd}>Add</Button>
                        </td>
                        <td>
                            <Button onClick={onClickEdit}>Edit</Button>
                        </td>
                        <td>
                            <Button onClick={onClickDelete}>Delete</Button>
                        </td>
                    </table>
                    <Table>
                        <TableHead>
                            <TableCell>Proxy</TableCell>
                            <TableCell>Proxy username</TableCell>
                            <TableCell>OK</TableCell>
                        </TableHead>
                        {accessNode}
                    </Table>
                </Box>
            </Modal>
        </>
    )
}

export default AccountModal;