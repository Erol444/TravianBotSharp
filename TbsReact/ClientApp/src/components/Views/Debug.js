import Grid from '@mui/material/Grid';

const Debug = ({ taskTable, logBoard }) => {
    return (
        <>
            <Grid container spacing={2}>
                <Grid item xs={6}>
                    {taskTable}
                </Grid>
                <Grid item xs={6}>
                    {logBoard}
                </Grid>
            </Grid>

        </>
    )
}

export default Debug;