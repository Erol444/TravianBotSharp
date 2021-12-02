import { useEffect, useState } from 'react';
import { Modal, Button, Box, Typography, Input, Table, TableHead, TableCell } from '@mui/material';

import { getAccount, addAccount } from '../../../api/Accounts/Account';
import { getAccesses } from '../../../api/Accounts/Access';
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

const AccountModal = ({ editMode = false, accID = -1, setAccID}) => {
    const [selected, setSelected] = useState(-1);
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

    useEffect(() => {
        if (selected !== -1) {
            setPassword(accesses[selected].password)
            setProxyIP(accesses[selected].proxy.ip)
            setProxyPort(accesses[selected].proxy.port)
            setProxyUsername(accesses[selected].proxy.username)
            setProxyPassword(accesses[selected].proxy.password)
        }
    }, [accesses, selected]);

    useEffect(() => {
        if (accID !== -1) {

            const fetchAccount = async () => {
                const getPromise = [getAccount(accID), getAccesses(accID)];
                const [{ name, serverUrl }, accesses] = await Promise.all(getPromise);
                setUsername(name)
                setServer(serverUrl)
                setAccesses(accesses)
            }

            fetchAccount();
        }
    }, [accID])


    const handleOpen = () => {
        if (editMode === true && accID === -1) {
            alert("Cannot edit if you didn't choose account to edit")
        }
        else {
            setOpen(true);
        }
    }
    const handleClose = () => {
        console.log("Setaccid", setAccID)
        setAccID(-1)
        setOpen(false);
    }
    const onClickTable = (access) => {
        setSelected(access.id)
    };
    const onClickAdd = async () => {
        const newAccess = { id: accesses.length, password: password, proxy: { ip: proxyIP, port: proxyPort, username: proxyUsername, password: proxyPassword, ok: false } }
        setAccesses(old => [...old, newAccess])
    }
    const onClickEdit = () => {
        const newAccess = { id: selected, password: password, proxy: { ip: proxyIP, port: proxyPort, username: proxyUsername, password: proxyPassword, ok: false } }
        setAccesses((old) => { old[selected] = newAccess; return old })
    }
    const onClickDelete = () => {
        setAccesses((accesses) => accesses.filter(access => access.id !== selected))
    }

    const onClickAddAccount = async () => {
        const data = { account: { name: username, serverUrl: server }, accesses: accesses };
        await addAccount(data)
        console.log(data);
        handleClose();
    }

    return (
        <>
            <Button onClick={handleOpen}>{editMode === false ? "Add account" : "Edit"} </Button>
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
                        <tbody>
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
                                    <Input type="password" value={proxyPassword} onInput={e => setProxyPassword(e.target.value)} />
                                </td>
                            </tr>
                        </tbody>
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
                        <AccessRow accesses={accesses} handler={onClickTable} selected={selected} />
                    </Table>
                    <table width="100%">
                        <td>
                            <Button onClick={onClickAddAccount}>{editMode === true ? "Edit account" : "Add account"} </Button>
                        </td>
                        <td>
                            <Button onClick={handleClose}>Close</Button>
                        </td>
                    </table>
                </Box>
            </Modal>
        </>
    )
}

export default AccountModal;